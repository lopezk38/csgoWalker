﻿using System;
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
        public Feeder Feeder;
       
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
            consoleBox.Select();
            Feeder = new Feeder();
        }
    }
}
