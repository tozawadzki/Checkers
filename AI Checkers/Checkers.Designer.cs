﻿namespace AICheckers
{
    partial class Checkers
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
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.boardPanel2 = new AICheckers.BoardPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.boardPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 33;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // boardPanel2
            // 
            this.boardPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boardPanel2.Controls.Add(this.button1);
            this.boardPanel2.Location = new System.Drawing.Point(16, 15);
            this.boardPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.boardPanel2.Name = "boardPanel2";
            this.boardPanel2.Size = new System.Drawing.Size(533, 492);
            this.boardPanel2.TabIndex = 0;
            this.boardPanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.boardPanel2_Paint);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(511, 104);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(8, 8);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 522);
            this.Controls.Add(this.boardPanel2);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormMain";
            this.Text = "AI Checkers";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.boardPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerRefresh;
        private BoardPanel boardPanel2;
        private System.Windows.Forms.Button button1;
    }
}

