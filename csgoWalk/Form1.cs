using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace csgoWalk
{
    public partial class walkerWindow : Form
    {
        public Feeder Feeder;
        public IntPtr HWND;

        public walkerWindow()
        {
            InitializeComponent();
            ConsoleAddLine("Initialized");
        }

        public void ConsoleAddLine(string line)
        {
            consoleBox.AppendText(line);
            consoleBox.AppendText(Environment.NewLine);
        }

        private void walkerWindow_Shown(object sender, EventArgs e)
        {

            HWND = this.Handle;
            Console.WriteLine(HWND);
            consoleBox.Select();
            Feeder = new Feeder();
        }
    }
}
