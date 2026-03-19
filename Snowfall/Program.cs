using SnowfallVillage;
using System;
using System.Windows.Forms;

namespace Snowfall
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SnowfallFrom());
        }
    }
}