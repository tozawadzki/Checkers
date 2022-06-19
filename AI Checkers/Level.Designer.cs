namespace AICheckers
{
    partial class Level
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
            this.label1 = new System.Windows.Forms.Label();
            this.boardSizeComboBox = new System.Windows.Forms.ComboBox();
            this.levelComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Board size";
            // 
            // boardSizeComboBox
            // 
            this.boardSizeComboBox.FormattingEnabled = true;
            this.boardSizeComboBox.Items.AddRange(new object[] {
            "8",
            "12"});
            this.boardSizeComboBox.Location = new System.Drawing.Point(102, 45);
            this.boardSizeComboBox.Name = "boardSizeComboBox";
            this.boardSizeComboBox.Size = new System.Drawing.Size(121, 21);
            this.boardSizeComboBox.TabIndex = 1;
            // 
            // levelComboBox
            // 
            this.levelComboBox.FormattingEnabled = true;
            this.levelComboBox.Items.AddRange(new object[] {
            "MinMax Easy",
            "MinMax Hard",
            "Monte Carlo"});
            this.levelComboBox.Location = new System.Drawing.Point(102, 83);
            this.levelComboBox.Name = "levelComboBox";
            this.levelComboBox.Size = new System.Drawing.Size(121, 21);
            this.levelComboBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Level";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(127, 116);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Level
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 151);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.levelComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.boardSizeComboBox);
            this.Controls.Add(this.label1);
            this.Name = "Level";
            this.Text = "Level";
            this.Load += new System.EventHandler(this.Level_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox boardSizeComboBox;
        private System.Windows.Forms.ComboBox levelComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}