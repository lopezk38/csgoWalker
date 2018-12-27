using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace csgoWalk
{
    public partial class walkerWindow : Form
    {
        public readonly Feeder Feeder;

        public walkerWindow()
        {
            InitializeComponent();
            ConsoleAddLine("Initialized");
            //Feeder = new Feeder();
        }

        public void ConsoleAddLine(string line)
        {
            consoleBox.AppendText(line);
            consoleBox.AppendText(Environment.NewLine);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
