using System;
using System.Threading;
using System.Windows.Forms;

namespace AICheckers
{
    public partial class Level : Form
    {
        Thread thread;
        public Level()
        {
            InitializeComponent();
        }

        private void Level_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int boardSize = Convert.ToInt32(boardSizeComboBox.Text);
            string level = levelComboBox.Text;
            SetGameConfig(boardSize, level);
            this.Close();
            thread = new Thread(OpenNewForm);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void SetGameConfig(int boardSize, string level)
        {
            if(boardSize == 12)
            {
                Constants.BOARD_SIZE = boardSize;
                Constants.COMPUTER_ROWS = 8;
            }

            if(level == "easy")
            {
                Constants.WEIGHT_KINGATRISK = 7;
                Constants.WEIGHT_ATRISK = 5;
                Constants.WEIGHT_CAPTUREMULTI = 4;
                Constants.WEIGHT_CAPTUREDOUBLE = 2;
                Constants.WEIGHT_CAPTUREPIECE = 5;
                Constants.WEIGHT_CAPTUREKING = 5;
            }
        }

        private void OpenNewForm(object obj)
        {
            Application.Run(new Checkers());
        }

    }
}
