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
            this.upBind = new System.Windows.Forms.Button();
            this.downBind = new System.Windows.Forms.Button();
            this.rightBind = new System.Windows.Forms.Button();
            this.leftBind = new System.Windows.Forms.Button();
            this.upBindHint = new System.Windows.Forms.ToolTip(this.components);
            this.bindLeftHint = new System.Windows.Forms.ToolTip(this.components);
            this.bindDownHint = new System.Windows.Forms.ToolTip(this.components);
            this.bindRightHint = new System.Windows.Forms.ToolTip(this.components);
            this.bindGroup = new System.Windows.Forms.GroupBox();
            this.loadButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.consoleBox = new System.Windows.Forms.TextBox();
            this.consoleOutputBox = new System.Windows.Forms.GroupBox();
            this.bindGroup.SuspendLayout();
            this.consoleOutputBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // upBind
            // 
            this.upBind.Location = new System.Drawing.Point(62, 19);
            this.upBind.Name = "upBind";
            this.upBind.Size = new System.Drawing.Size(50, 50);
            this.upBind.TabIndex = 0;
            this.upBind.Text = "Up";
            this.upBind.UseVisualStyleBackColor = true;
            // 
            // downBind
            // 
            this.downBind.Location = new System.Drawing.Point(62, 75);
            this.downBind.Name = "downBind";
            this.downBind.Size = new System.Drawing.Size(50, 50);
            this.downBind.TabIndex = 1;
            this.downBind.Text = "Down";
            this.downBind.UseVisualStyleBackColor = true;
            // 
            // rightBind
            // 
            this.rightBind.Location = new System.Drawing.Point(118, 75);
            this.rightBind.Name = "rightBind";
            this.rightBind.Size = new System.Drawing.Size(50, 50);
            this.rightBind.TabIndex = 2;
            this.rightBind.Text = "Right";
            this.rightBind.UseVisualStyleBackColor = true;
            // 
            // leftBind
            // 
            this.leftBind.Location = new System.Drawing.Point(6, 75);
            this.leftBind.Name = "leftBind";
            this.leftBind.Size = new System.Drawing.Size(50, 50);
            this.leftBind.TabIndex = 3;
            this.leftBind.Text = "Left";
            this.leftBind.UseVisualStyleBackColor = true;
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
            this.bindGroup.Controls.Add(this.loadButton);
            this.bindGroup.Controls.Add(this.saveButton);
            this.bindGroup.Controls.Add(this.leftBind);
            this.bindGroup.Controls.Add(this.rightBind);
            this.bindGroup.Controls.Add(this.downBind);
            this.bindGroup.Controls.Add(this.upBind);
            this.bindGroup.Location = new System.Drawing.Point(12, 149);
            this.bindGroup.Name = "bindGroup";
            this.bindGroup.Size = new System.Drawing.Size(343, 131);
            this.bindGroup.TabIndex = 4;
            this.bindGroup.TabStop = false;
            this.bindGroup.Text = "Set keybinds...";
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(179, 75);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(158, 50);
            this.loadButton.TabIndex = 5;
            this.loadButton.Text = "Load keybinds";
            this.loadButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(179, 19);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(158, 50);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save keybinds";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // consoleBox
            // 
            this.consoleBox.BackColor = System.Drawing.SystemColors.MenuText;
            this.consoleBox.ForeColor = System.Drawing.Color.LimeGreen;
            this.consoleBox.Location = new System.Drawing.Point(6, 15);
            this.consoleBox.Multiline = true;
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.ReadOnly = true;
            this.consoleBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.consoleBox.Size = new System.Drawing.Size(331, 105);
            this.consoleBox.TabIndex = 5;
            // 
            // consoleOutputBox
            // 
            this.consoleOutputBox.Controls.Add(this.consoleBox);
            this.consoleOutputBox.Location = new System.Drawing.Point(12, 12);
            this.consoleOutputBox.Name = "consoleOutputBox";
            this.consoleOutputBox.Size = new System.Drawing.Size(343, 126);
            this.consoleOutputBox.TabIndex = 6;
            this.consoleOutputBox.TabStop = false;
            this.consoleOutputBox.Text = "Console";
            // 
            // walkerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 291);
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

        private System.Windows.Forms.Button upBind;
        private System.Windows.Forms.Button downBind;
        private System.Windows.Forms.Button rightBind;
        private System.Windows.Forms.Button leftBind;
        private System.Windows.Forms.ToolTip upBindHint;
        private System.Windows.Forms.ToolTip bindLeftHint;
        private System.Windows.Forms.ToolTip bindDownHint;
        private System.Windows.Forms.ToolTip bindRightHint;
        private System.Windows.Forms.GroupBox bindGroup;
        private System.Windows.Forms.TextBox consoleBox;
        private System.Windows.Forms.GroupBox consoleOutputBox;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
    }
}

