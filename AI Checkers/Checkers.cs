using System;
using System.Windows.Forms;

namespace AICheckers
{
    public partial class Checkers : Form
    {
        public Checkers()
        {
            InitializeComponent();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            boardPanel2.Invalidate();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void boardPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
