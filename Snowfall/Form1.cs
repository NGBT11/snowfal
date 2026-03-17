using Snowfall;
using Snowfall.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnowfallVillage
{
    public partial class Form1 : Form
    {
        private List<Snowflake> snowflakes = new List<Snowflake>();
        private List<Bitmap> snowflakeVariants = new List<Bitmap>();

        private Bitmap villageBackground;
        private Bitmap snowflakeSource;

        private Timer timer;
        private Random random = new Random();
        private Bitmap buffer;

        private const float MinScale = 0.35f;
        private const float MaxScale = 1f;
        private const double MinSpeed = 1.5;
        private const double MaxSpeed = 6.5;
        private const int SnowflakeCount = 100;



        public Form1()
        {
            InitializeComponent();
            SetupForm();
            LoadImagesFromResources();
            Shown += Form1_Shown;
        }

        private void SetupForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.Black;

            KeyPreview = true;
            KeyDown += (s, e) => Application.Exit();
        }

        private void LoadImagesFromResources()
        {
            try
            {
                // ЗАГРУЗКА ЧЕРЕЗ PROPERTIES.RESOURCES
                villageBackground = Resources.village;
                snowflakeSource = Resources.snowflake;

                if (villageBackground == null)
                    MessageBox.Show("village.jpg не найден в ресурсах");
                if (snowflakeSource == null)
                    MessageBox.Show("snowflake.png не найден в ресурсах");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

            buffer = new Bitmap(Width, Height);
            CreateSnowflakes(100);
            StartAnimation();
        }

        private void CreateSnowflakes(int count)
        {
            snowflakes.Clear();

            for (int i = 0; i < count; i++)
            {
                float scale = (float)(MinScale + (MaxScale - MinScale) * random.NextDouble());
                double speed = MinSpeed + (MaxSpeed - MinSpeed) * ((scale - MinScale) / (MaxScale - MinScale));

                snowflakes.Add(new Snowflake
                {
                    X = random.Next(0, Width),
                    Y = random.Next(-Height, 0),
                    Scale = scale,
                    Speed = speed
                });
            }
        }



        private void StartAnimation()
        {
            timer = new Timer();
            timer.Interval = 40;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateSnowflakes();
            DrawScene();
        }

        private void UpdateSnowflakes()
        {
            for (int i = 0; i < snowflakes.Count; i++)
            {
                var flake = snowflakes[i];
                flake.Y += flake.Speed;

                if (flake.Y > Height)
                {
                    flake.Y = random.Next(-200, -50);
                    flake.X = random.Next(0, Width);
                    flake.Scale = (float)(MinScale + (MaxScale - MinScale) * random.NextDouble());
                    flake.Speed = MinSpeed + (MaxSpeed - MinSpeed) * ((flake.Scale - MinScale) / (MaxScale - MinScale));
                }

                snowflakes[i] = flake;
            }
        }


        private void DrawScene()
        {
            using (Graphics g = Graphics.FromImage(buffer))
            {
                if (villageBackground != null)
                {
                    g.DrawImage(villageBackground, 0, 0, Width, Height);
                }
                else
                {
                    g.Clear(Color.Black);
                }

                foreach (var flake in snowflakes)
                {
                    if (flake.Y > -50 && flake.Y < Height + 50)
                    {
                        int w = (int)(snowflakeSource.Width * flake.Scale);
                        int h = (int)(snowflakeSource.Height * flake.Scale);

                        g.DrawImage(snowflakeSource, (float)flake.X, (float)flake.Y, w, h);
                    }
                }
            }

            using (Graphics g = CreateGraphics())
            {
                g.DrawImage(buffer, 0, 0);
            }
        }
    }
}

