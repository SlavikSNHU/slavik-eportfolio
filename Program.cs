using System;
using System.Windows.Forms;

namespace SNHU_CS499_SlavikPlamadyala
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConfigurationWindow());
        }
    }
}
