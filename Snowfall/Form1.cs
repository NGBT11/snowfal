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

        public Form1()
        {
            InitializeComponent();
            SetupForm();
            LoadImages();
            Shown += Form1_Shown;
        }

        private void SetupForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.Black;
            DoubleBuffered = false;

            KeyPreview = true;
            KeyDown += (s, e) => Application.Exit();
        }

        private void LoadImages()
        {
            try
            {
                villageBackground = new Bitmap("village.jpg");
            }
            catch
            {
                villageBackground = null;
            }

            try
            {
                snowflakeSource = new Bitmap("snowflake.png");
            }
            catch
            {
                snowflakeSource = null;
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
            timer.Interval = 50;
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
                    g.DrawImage(villageBackground, 0, 0, Width, Height);
                else
                    g.Clear(Color.Black);

                foreach (var flake in snowflakes)
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
