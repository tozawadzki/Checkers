namespace AICheckers
{
    public static class Constants
    {

        private static int _boardSize = 8;
        private static int _userRows = 3;
        private static int _computerRows = 4;
        private static bool _computerCanMove = true;
        private static int _capturePiece = 2;
        private static int _captureKing = 1;
        private static int _captureDouble = 5;
        private static int _captureMulti = 10;
        private static int _atRisk = 3;
        private static int _kingAtRisk = 4;
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

        //Waga ruchów ofensywnych
        public static int WEIGHT_CAPTUREPIECE
        {
            get { return _capturePiece; }
            set { _capturePiece = value; }
        }
        public static int WEIGHT_CAPTUREKING
        {
            get { return _captureKing; }
            set { _captureKing = value; }
        }

        public static int WEIGHT_CAPTUREDOUBLE
        {
            get { return _captureDouble; }
            set { _captureDouble = value; }
        }
        public static int WEIGHT_CAPTUREMULTI
        {
            get {  return _captureMulti; } 
            set { _captureMulti = value; }
        }

        public static int WEIGHT_ATRISK
        {
            get { return _atRisk; }
            set { _atRisk = value; }
        }

        public static int WEIGHT_KINGATRISK
        {
            get { return _kingAtRisk; }
            set { _kingAtRisk = value; }
        }

        //Maksymalne "zagięcie", określamy w każdym drzewie behawioralnym
        public static int AI_MAXPLYLEVEL = 2;
    }
}
