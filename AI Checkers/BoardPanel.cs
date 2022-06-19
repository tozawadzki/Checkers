using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using AICheckers.Properties;
using System;

namespace AICheckers
{
    class BoardPanel : Panel
    {

        IAI AI = null;

        //Zainicjalizowanie kwadracików kolorami
        private readonly Color lightSquare = Color.LightGray;

        //Upload obrazków 
        private readonly Image redChecker = Resources.checkerred;
        private readonly Image blackChecker = Resources.checkerblack;
        private readonly Image redCheckerKing = Resources.checkerredking;
        private readonly Image blackCheckerKing = Resources.checkerblackking;

        //Określenie tury 
        private CheckerColor turn = CheckerColor.Black;

        //Ustawienie domyślnych wartości dla punktów
        private Point oldP = new Point(-1, -1);
        private Point currentP = new Point(-1, -1);
        private Point newP = new Point(-1, -1);
        private Point selectedChecker = new Point(-1, -1);

        List<Move> possibleMoves = new List<Move>();

        //Animacje
        private readonly Square[,] Board = new Square[Constants.BOARD_SIZE, Constants.BOARD_SIZE];
        private int squareWidth = 0;

        //Inicjalizacja panelu
        public BoardPanel()
            : base()
        {
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.Opaque |
                ControlStyles.AllPaintingInWmPaint, true);
            this.ResizeRedraw = true;

            //INICJALIZUJEMY PLANSZE
            for (int i = 0; i < Constants.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Constants.BOARD_SIZE; j++)
                {
                    Board[i, j] = new Square();
                    Board[i, j].Color = CheckerColor.Empty;
                }
            }

            //OKREŚLAMY CZYJE WARCABY SĄ PO JAKIEJ STRONIE
            for (int i = 0; i < Constants.BOARD_SIZE; i++)
            {
                int offset = 0;

                if (i % 2 != 0)
                    offset++;

                //Przygotowanie warcabów (rozmieszczenie na planszy czarnych i czerwonych)
                for (int j = offset; j < Constants.BOARD_SIZE; j += 2)
                {
                    if (i < Constants.USER_ROWS)
                        Board[i, j].Color = CheckerColor.Red;
                    if (i > Constants.COMPUTER_ROWS)
                        Board[i, j].Color = CheckerColor.Black;
                }
            }

            switch(Constants.AI_AlGORITHM)
            {
                case "MinMax Easy":
                case "MinMax Hard":
                    AI = new AI_Tree();
                    break;
                case "Monte Carlo":
                    AI = new MonteCarlo();
                    break;
            }
            

            //PRZYDZIELAMY KOMPUTEROWI CZARNE WARCABY
            AI.Color = CheckerColor.Black;

            //Przydzielenie tury
            ChangeTurn();
        }

        private void ChangeTurn()
        {
            if (turn == CheckerColor.Red)
                turn = CheckerColor.Black;
            else
                turn = CheckerColor.Red;

            if (AI != null && AI.Color == turn)
            {
                Move AIMove = AI.Process(Board);
                MoveChecker(AIMove);
            }
        }

        private void Reset(Point square)
        {
            Board[square.Y, square.X].Color = CheckerColor.Empty;
            Board[square.Y, square.X].King = false;
        }

        private void MoveChecker(Move move)
        {
            // Ustalamy property warcaba po ruchu, aby zostały takie same
            Board[move.Destination.Y, move.Destination.X].Color = Board[move.Source.Y, move.Source.X].Color;
            // Dzięki temu zapobiegamy sytuacji, w której po ruchu nagle warcab traci miano Króla
            Board[move.Destination.Y, move.Destination.X].King = Board[move.Source.Y, move.Source.X].King;
            Reset(move.Source);

            foreach (Point point in move.Captures)
                Reset(point);

            selectedChecker.X = -1;
            selectedChecker.Y = -1;

            // Pola królewskie
            if ((move.Destination.Y == Constants.BOARD_SIZE - 1 && Board[move.Destination.Y, move.Destination.X].Color == CheckerColor.Red)
                || (move.Destination.Y == 0 &&
                    Board[move.Destination.Y, move.Destination.X].Color == CheckerColor.Black))
            {
                Board[move.Destination.Y, move.Destination.X].King = true;
            }

            // Czyścimy potencjalne ruchy
            possibleMoves.Clear();

            //Ustalamy nowe współrzędne warcaba
            oldP.X = move.Source.Y * squareWidth;
            oldP.Y = move.Source.X * squareWidth;
            newP.X = move.Destination.Y * squareWidth;
            newP.Y = move.Destination.X * squareWidth;
            currentP = oldP;

            this.Invalidate();

            //Zmiana tury
            ChangeTurn();

        }

        /// <summary>
        /// Przeciężenie wirtualnej metody OnMouseClick, w celu nadania jej specyficznej funkcjonalności
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            int clickedX = (int) (((double) e.X / (double) Width) * Constants.BOARD_SIZE);
            int clickedY = (int) (((double) e.Y / (double) Height) * Constants.BOARD_SIZE);

            //Tworzymy nowy punkt kliknięcia
            Point clickedP = new Point(clickedX, clickedY);

            //Sprawdzamy ruch gracza
            if (Board[clickedY, clickedX].Color != CheckerColor.Empty
                && Board[clickedY, clickedX].Color != turn)
                return;

            //Sprawdzamy, czy gracz się porusza czy tylko zmienia warcaba
            List<Move> matches = possibleMoves.Where(m => m.Destination == clickedP).ToList<Move>();
            if (matches.Count > 0)
            {
                //Poruszamy się warcabem we wskazane miejsce
                MoveChecker(matches[0]);
            }
            else if (Board[clickedY, clickedX].Color != CheckerColor.Empty)
            {
                //Zmieniamy warcaba
                selectedChecker.X = clickedX;
                selectedChecker.Y = clickedY;
                possibleMoves.Clear();

                Move[] OpenSquares = Minimax.GetOpenSquares(Board, selectedChecker);
                possibleMoves.AddRange(OpenSquares);

                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);
            e.Graphics.Clear(lightSquare);

            //Rysujemy boarda
            squareWidth = (Width) / Constants.BOARD_SIZE;
            for (int c = 0; c < Width; c += squareWidth)
            {
                int offset = 0;
                if ((c / squareWidth) % 2 != 0)
                {
                    offset += squareWidth;
                }

                for (int i = offset; i < Width; i += (squareWidth * 2))
                {
                    e.Graphics.FillRectangle(Brushes.DarkKhaki, c, i, squareWidth, squareWidth);
                }
            }

            //Pokazanie możliwych ruchów
            foreach (Move move in possibleMoves)
            {
                e.Graphics.FillRectangle(Brushes.Red, move.Destination.X * squareWidth,
                    move.Destination.Y * squareWidth, squareWidth, squareWidth);
            }

            if (selectedChecker.X >= 0 && selectedChecker.Y >= 0)
            {
                e.Graphics.FillRectangle(Brushes.AntiqueWhite, selectedChecker.X * squareWidth,
                    selectedChecker.Y * squareWidth, squareWidth, squareWidth);
            }

            //Rysujemy planszę
            e.Graphics.DrawRectangle(Pens.DarkKhaki,
                e.ClipRectangle.Left,
                e.ClipRectangle.Top,
                e.ClipRectangle.Width - 1,
                e.ClipRectangle.Height - 1);

            //Lepsza jakość grafiki
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            //Rysujemy warcaby poprzez dostarczone obrazki
            for (int i = 0; i < Constants.BOARD_SIZE; i++)
            {
                for (int j = 0;  j < Constants.BOARD_SIZE; j++)
                {
                    if (Board[i, j].Color == CheckerColor.Red)
                    {
                        if (Board[i, j].King)
                        {
                            e.Graphics.DrawImage(redCheckerKing,
                                new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                        else
                        {
                            e.Graphics.DrawImage(redChecker,
                                new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                    }
                    else if (Board[i, j].Color == CheckerColor.Black)
                    {
                        if (Board[i, j].King)
                        {
                            e.Graphics.DrawImage(blackCheckerKing,
                                new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                        else
                        {
                            e.Graphics.DrawImage(blackChecker,
                                new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                    }
                    
                }
            }

            var redCheckers = Board.Cast<Square>().Where(x => x.Color == CheckerColor.Red);
            var blackCheckers = Board.Cast<Square>().Where(x => x.Color == CheckerColor.Black);

            //Koniec gry
            if (!redCheckers.Any() || !blackCheckers.Any())
                //Zamknięcie aplikacji
                Environment.Exit(0);
        }
    }
}