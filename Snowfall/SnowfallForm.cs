using Snowfall;
using Snowfall.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnowfallVillage
{
    /// <summary>
    /// Класс формы снегопада
    /// </summary>
    public partial class SnowfallFrom : Form
    {
        private readonly List<Snowflake> Snowflakes = new List<Snowflake>();

        private Bitmap VillageBackground;
        private Bitmap SnowflakeSource;

        private Timer Timer;
        private readonly Random Random = new Random();
        private Bitmap Buffer;

        private const float MinScale = 0.35f;
        private const float MaxScale = 1f;
        private const double MinSpeed = 1.5;
        private const double MaxSpeed = 6.5;
        private const int SnowflakeCount = 100;
        private const int HighestSpawnPosition = -200;
        private const int LowestSpawnPosition = -50;
        private const int VisibilityPadding = 50;


        /// <summary>
        /// Ctor
        /// </summary>
        public SnowfallFrom()
        {
            InitializeComponent();
            LoadImagesFromResources();
            Shown += Form1_Shown;
        }

        private void LoadImagesFromResources()
        {
            try
            {
                VillageBackground = Resources.village;
                SnowflakeSource = Resources.snowflake;

                if (VillageBackground == null)
                {
                    MessageBox.Show("village.jpg не найден в ресурсах");
                }
                if (SnowflakeSource == null)
                {
                    MessageBox.Show("snowflake.png не найден в ресурсах");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Buffer = new Bitmap(Width, Height);
            CreateSnowflakes(SnowflakeCount);
            StartAnimation();
        }

        private void CreateSnowflakes(int count)
        {
            Snowflakes.Clear();

            for (var flakeCounter = 0; flakeCounter < count; flakeCounter++)
            {
                var scale = (float)(MinScale + (MaxScale - MinScale) * Random.NextDouble());
                var speed = MinSpeed + (MaxSpeed - MinSpeed) * ((scale - MinScale) / (MaxScale - MinScale));

                Snowflakes.Add(new Snowflake
                {
                    PosX = Random.Next(0, Width),
                    PosY = Random.Next(-Height, 0),
                    Scale = scale,
                    Speed = speed
                });
            }
        }

        private void StartAnimation()
        {
            Timer = new Timer
            {
                Interval = 40
            };
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateSnowflakes();
            DrawScene();
        }

        private void UpdateSnowflakes()
        {
            for (int flakeCounter = 0; flakeCounter < Snowflakes.Count; flakeCounter++)
            {
                var flake = Snowflakes[flakeCounter];
                flake.PosY += flake.Speed;

                if (flake.PosY > Height)
                {
                    flake.PosY = Random.Next(HighestSpawnPosition, LowestSpawnPosition);
                    flake.PosX = Random.Next(0, Width);
                    flake.Scale = (float)(MinScale + (MaxScale - MinScale) * Random.NextDouble());
                    flake.Speed = MinSpeed + (MaxSpeed - MinSpeed) * ((flake.Scale - MinScale) / (MaxScale - MinScale));
                }

                Snowflakes[flakeCounter] = flake;
            }
        }


        private void DrawScene()
        {
            using (Graphics graphic = Graphics.FromImage(Buffer))
            {
                if (VillageBackground != null)
                {
                    graphic.DrawImage(VillageBackground, 0, 0, Width, Height);
                }
                else
                {
                    graphic.Clear(Color.Black);
                }

                foreach (var flake in Snowflakes)
                {
                    if (flake.PosY > -VisibilityPadding && flake.PosY < Height + VisibilityPadding)
                    {
                        var width = (int)(SnowflakeSource.Width * flake.Scale);
                        var height = (int)(SnowflakeSource.Height * flake.Scale);

                        graphic.DrawImage(SnowflakeSource, (float)flake.PosX, (float)flake.PosY, width, height);
                    }
                }
            }

            using (Graphics graphic = CreateGraphics())
            {
                graphic.DrawImage(Buffer, 0, 0);
            }
        }

        private void SnowfallFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            Application.Exit();
        }
    }
}

