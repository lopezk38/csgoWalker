namespace csgoWalk
{
    partial class walkerWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.leftBind = new System.Windows.Forms.Button();
            this.downBind = new System.Windows.Forms.Button();
            this.rightBind = new System.Windows.Forms.Button();
            this.upBind = new System.Windows.Forms.Button();
            this.upBindHint = new System.Windows.Forms.ToolTip(this.components);
            this.bindLeftHint = new System.Windows.Forms.ToolTip(this.components);
            this.bindDownHint = new System.Windows.Forms.ToolTip(this.components);
            this.bindRightHint = new System.Windows.Forms.ToolTip(this.components);
            this.bindGroup = new System.Windows.Forms.GroupBox();
            this.consoleBox = new System.Windows.Forms.TextBox();
            this.consoleOutputBox = new System.Windows.Forms.GroupBox();
            this.bindGroup.SuspendLayout();
            this.consoleOutputBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // leftBind
            // 
            this.leftBind.Location = new System.Drawing.Point(77, 14);
            this.leftBind.Name = "leftBind";
            this.leftBind.Size = new System.Drawing.Size(50, 50);
            this.leftBind.TabIndex = 0;
            this.leftBind.Text = "Left";
            this.leftBind.UseVisualStyleBackColor = true;
            // 
            // downBind
            // 
            this.downBind.Location = new System.Drawing.Point(77, 70);
            this.downBind.Name = "downBind";
            this.downBind.Size = new System.Drawing.Size(50, 50);
            this.downBind.TabIndex = 1;
            this.downBind.Text = "Down";
            this.downBind.UseVisualStyleBackColor = true;
            // 
            // rightBind
            // 
            this.rightBind.Location = new System.Drawing.Point(133, 70);
            this.rightBind.Name = "rightBind";
            this.rightBind.Size = new System.Drawing.Size(50, 50);
            this.rightBind.TabIndex = 2;
            this.rightBind.Text = "Right";
            this.rightBind.UseVisualStyleBackColor = true;
            // 
            // upBind
            // 
            this.upBind.Location = new System.Drawing.Point(21, 70);
            this.upBind.Name = "upBind";
            this.upBind.Size = new System.Drawing.Size(50, 50);
            this.upBind.TabIndex = 3;
            this.upBind.Text = "Up";
            this.upBind.UseVisualStyleBackColor = true;
            // 
            // upBindHint
            // 
            this.upBindHint.ToolTipTitle = "Click to bind key...";
            // 
            // bindLeftHint
            // 
            this.bindLeftHint.ToolTipTitle = "Click to bind key...";
            // 
            // bindDownHint
            // 
            this.bindDownHint.ToolTipTitle = "Click to bind key...";
            // 
            // bindRightHint
            // 
            this.bindRightHint.ToolTipTitle = "Click to bind key...";
            // 
            // bindGroup
            // 
            this.bindGroup.Controls.Add(this.upBind);
            this.bindGroup.Controls.Add(this.rightBind);
            this.bindGroup.Controls.Add(this.downBind);
            this.bindGroup.Controls.Add(this.leftBind);
            this.bindGroup.Location = new System.Drawing.Point(12, 149);
            this.bindGroup.Name = "bindGroup";
            this.bindGroup.Size = new System.Drawing.Size(205, 131);
            this.bindGroup.TabIndex = 4;
            this.bindGroup.TabStop = false;
            this.bindGroup.Text = "Set keybinds...";
            // 
            // consoleBox
            // 
            this.consoleBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.consoleBox.ForeColor = System.Drawing.Color.LimeGreen;
            this.consoleBox.Location = new System.Drawing.Point(6, 15);
            this.consoleBox.Multiline = true;
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.ReadOnly = true;
            this.consoleBox.Size = new System.Drawing.Size(192, 105);
            this.consoleBox.TabIndex = 5;
            // 
            // consoleOutputBox
            // 
            this.consoleOutputBox.Controls.Add(this.consoleBox);
            this.consoleOutputBox.Location = new System.Drawing.Point(12, 12);
            this.consoleOutputBox.Name = "consoleOutputBox";
            this.consoleOutputBox.Size = new System.Drawing.Size(204, 126);
            this.consoleOutputBox.TabIndex = 6;
            this.consoleOutputBox.TabStop = false;
            this.consoleOutputBox.Text = "Console";
            // 
            // walkerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(229, 291);
            this.Controls.Add(this.consoleOutputBox);
            this.Controls.Add(this.bindGroup);
            this.MaximizeBox = false;
            this.Name = "walkerWindow";
            this.Text = "CS:GO Walker";
            this.Shown += new System.EventHandler(this.walkerWindow_Shown);
            this.bindGroup.ResumeLayout(false);
            this.consoleOutputBox.ResumeLayout(false);
            this.consoleOutputBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button leftBind;
        private System.Windows.Forms.Button downBind;
        private System.Windows.Forms.Button rightBind;
        private System.Windows.Forms.Button upBind;
        private System.Windows.Forms.ToolTip upBindHint;
        private System.Windows.Forms.ToolTip bindLeftHint;
        private System.Windows.Forms.ToolTip bindDownHint;
        private System.Windows.Forms.ToolTip bindRightHint;
        private System.Windows.Forms.GroupBox bindGroup;
        private System.Windows.Forms.TextBox consoleBox;
        private System.Windows.Forms.GroupBox consoleOutputBox;
    }
}

