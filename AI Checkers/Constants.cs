namespace AICheckers
{
    public static class Constants
    {

        private static int _boardSize = 8;
        private static int _userRows = 1;
        private static int _computerRows = 4;
        private static bool _computerCanMove = true;
        public static int BOARD_SIZE
        {
            get { return _boardSize; }
            set { _boardSize = value; }
        }

        public static int USER_ROWS
        {
            get { return _userRows; }
            set { _userRows = value; }
        }

        public static int COMPUTER_ROWS
        {
            get { return _computerRows; }
            set { _computerRows = value; }
        }

        public static bool COMPUTER_CAN_MOVE
        {
            get { return _computerCanMove; }
            set { _computerCanMove = value; }
        }

        //Maksymalne "zagięcie", określamy w każdym drzewie behawioralnym
        public static int AI_MAXPLYLEVEL = 2;

        //Waga ruchów ofensywnych
        public static int WEIGHT_CAPTUREPIECE = 2;
        public static int WEIGHT_CAPTUREKING = 1;
        public static int WEIGHT_CAPTUREDOUBLE = 5;
        public static int WEIGHT_CAPTUREMULTI = 10;

        //Waga ruchów obronnych
        public static int WEIGHT_ATRISK = 3;
        public static int WEIGHT_KINGATRISK = 4;

        public static int USERS_ROWS = 3;


    }
}
