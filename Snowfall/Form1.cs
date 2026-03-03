using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Snowfall.Properties;

namespace SnowfallVillage
{
    public partial class Form1 : Form
    {
        private List<Snowflake> snowflakes = new List<Snowflake>();
        private List<Bitmap> snowflakeVariants = new List<Bitmap>();

        private Bitmap villageBackground;
        private Bitmap snowflakeSource; // БЫЛО: snowflakeImage

        private Timer timer;
        private Random random = new Random();

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
            DoubleBuffered = false; // По заданию

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
            if (snowflakeSource == null) 
            {
                MessageBox.Show("Не найдена snowflake.png");
                Close();
                return;
            }

            PrepareSnowflakeVariants();
            CreateSnowflakes(100);
            StartAnimation();
        }

        private void PrepareSnowflakeVariants()
        {
            snowflakeVariants.Clear();

            float[] scales = { 0.15f, 0.25f, 0.35f };

            foreach (float scale in scales)
            {
                int w = (int)(snowflakeSource.Width * scale);
                int h = (int)(snowflakeSource.Height * scale);

                Bitmap bmp = new Bitmap(w, h);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(snowflakeSource, 0, 0, w, h);
                }

                snowflakeVariants.Add(bmp);
            }
        }

        private void CreateSnowflakes(int count)
        {
            snowflakes.Clear();

            for (int i = 0; i < count; i++)
            {
                Bitmap img = snowflakeVariants[random.Next(snowflakeVariants.Count)];

                snowflakes.Add(new Snowflake
                {
                    X = random.Next(0, Width),
                    Y = random.Next(-Height, 0),
                    Speed = 0.8 + img.Width * 0.05,
                    Image = img
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
                    Bitmap img = snowflakeVariants[random.Next(snowflakeVariants.Count)];

                    flake.Y = random.Next(-200, -50); 
                    flake.X = random.Next(0, Width);
                    flake.Image = img;
                    flake.Speed = 0.8 + img.Width * 0.05;
                }

                snowflakes[i] = flake;
            }
        }

        private void DrawScene()
        {
            using (Graphics g = CreateGraphics())
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
                        g.DrawImage(
                            flake.Image,
                            (float)flake.X,
                            (float)flake.Y
                        );
                    }
                }


            }
        }




        public struct Snowflake
        {
            public double X;
            public double Y;
            public double Speed;
            public Bitmap Image;
        }
    }
}
