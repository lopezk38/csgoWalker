using System;
using System.Windows.Forms;

namespace csgoWalk
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static walkerWindow walkerWindowObj;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            walkerWindowObj = new walkerWindow();
            Application.Run(walkerWindowObj);
        }
    }
}
