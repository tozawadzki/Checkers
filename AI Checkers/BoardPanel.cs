using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using AICheckers.Properties;

namespace AICheckers
{
    class BoardPanel : Panel
    {

        IAI AI = null;

        //Zainicjalizowanie kwadracików kolorami
        private readonly Color darkSquare = Color.Chocolate;
        private readonly Color lightSquare = Color.LightGray;

        //Upload obrazków 
        private readonly Image redChecker = Resources.checkerred;
        private readonly Image blackChecker = Resources.checkerblack;
        private readonly Image redCheckerKing = Resources.checkerredking;
        private readonly Image blackCheckerKing = Resources.checkerblackking;

        //Określenie tury 
        private CheckerColour turn = CheckerColour.Black;

        //Ustawienie domyślnych wartości dla punktów
        private Point oldP = new Point(-1, -1);
        private Point currentP = new Point(-1, -1);
        private Point newP = new Point(-1, -1);
        private Point selectedChecker = new Point(-1, -1);

        List<Move> possibleMoves = new List<Move>();

        //Animacje
        private const int animationDuration = 1000; // 1000ms=1s
        private readonly Square[,] Board = new Square[8, 8];
        private int squareWidth = 0;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        //Inicjalizacja panelu
        public BoardPanel()
            : base()
        {
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.Opaque |
                ControlStyles.AllPaintingInWmPaint, true);
            this.ResizeRedraw = true;

            //INICJALIZUJEMY PLANSZE
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Board[i, j] = new Square();
                    Board[i, j].Colour = CheckerColour.Empty;
                }
            }

            //OKREŚLAMY CZYJE WARCABY SĄ PO JAKIEJ STRONIE
            for (int i = 0; i < 8; i++)
            {
                int offset = 0;

                if (i % 2 != 0)
                    offset++;

                //Przygotowanie warcabów (rozmieszczenie na planszy czarnych i czerwonych)
                for (int j = offset; j < 8; j += 2)
                {
                    if (i < 3)
                        Board[i, j].Colour = CheckerColour.Red;
                    if (i > 4)
                        Board[i, j].Colour = CheckerColour.Black;
                }
            }

            AI = new AI_Tree();

            //PRZYDZIELAMY KOMPUTEROWI CZARNE WARCABY
            AI.Colour = CheckerColour.Black;

            //Przydzielenie tury
            ChangeTurn();
        }

        private void ChangeTurn()
        {
            if (turn == CheckerColour.Red)
                turn = CheckerColour.Black;
            else
                turn = CheckerColour.Red;

            if (AI != null && AI.Colour == turn)
            {
                Move AIMove = AI.Process(Board);
                MoveChecker(AIMove);
            }
        }

        private void Reset(Point square)
        {
            Board[square.Y, square.X].Colour = CheckerColour.Empty;
            Board[square.Y, square.X].King = false;
        }

        private void MoveChecker(Move move)
        {
            // Ustalamy property warcaba po ruchu, aby zostały takie same
            Board[move.Destination.Y, move.Destination.X].Colour = Board[move.Source.Y, move.Source.X].Colour;
            // Dzięki temu zapobiegamy sytuacji, w której po ruchu nagle warcab traci miano Króla
            Board[move.Destination.Y, move.Destination.X].King = Board[move.Source.Y, move.Source.X].King;
            Reset(move.Source);

            foreach (Point point in move.Captures)
                Reset(point);

            selectedChecker.X = -1;
            selectedChecker.Y = -1;

            // Pola królewskie
            if ((move.Destination.Y == 7 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Red)
                || (move.Destination.Y == 0 &&
                    Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Black))
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
            int clickedX = (int) (((double) e.X / (double) Width) * 8.0d);
            int clickedY = (int) (((double) e.Y / (double) Height) * 8.0d);

            //Tworzymy nowy punkt kliknięcia
            Point clickedP = new Point(clickedX, clickedY);

            //Sprawdzamy ruch gracza
            if (Board[clickedY, clickedX].Colour != CheckerColour.Empty
                && Board[clickedY, clickedX].Colour != turn)
                return;

            //Sprawdzamy, czy gracz się porusza czy tylko zmienia warcaba
            List<Move> matches = possibleMoves.Where(m => m.Destination == clickedP).ToList<Move>();
            if (matches.Count > 0)
            {
                //Poruszamy się warcabem we wskazane miejsce
                MoveChecker(matches[0]);
            }
            else if (Board[clickedY, clickedX].Colour != CheckerColour.Empty)
            {
                //Zmieniamy warcaba
                selectedChecker.X = clickedX;
                selectedChecker.Y = clickedY;
                possibleMoves.Clear();

                Move[] OpenSquares = Utils.GetOpenSquares(Board, selectedChecker);
                possibleMoves.AddRange(OpenSquares);

                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);
            e.Graphics.Clear(lightSquare);

            //Rysujemy boarda
            squareWidth = (Width) / 8;
            for (int c = 0; c < Width; c += squareWidth)
            {
                int offset = 0;
                if ((c / squareWidth) % 2 != 0)
                {
                    offset += squareWidth;
                }

                for (int i = offset; i < Width; i += (squareWidth * 2))
                {
                    e.Graphics.FillRectangle(Brushes.DarkGray, c, i, squareWidth, squareWidth);
                }
            }

            //Pokazanie możliwych ruchów
            foreach (Move move in possibleMoves)
            {
                e.Graphics.FillRectangle(Brushes.PaleTurquoise, move.Destination.X * squareWidth,
                    move.Destination.Y * squareWidth, squareWidth, squareWidth);
            }

            if (selectedChecker.X >= 0 && selectedChecker.Y >= 0)
            {
                e.Graphics.FillRectangle(Brushes.PeachPuff, selectedChecker.X * squareWidth,
                    selectedChecker.Y * squareWidth, squareWidth, squareWidth);
            }

            //Rysujemy planszę
            e.Graphics.DrawRectangle(Pens.DarkGray,
                e.ClipRectangle.Left,
                e.ClipRectangle.Top,
                e.ClipRectangle.Width - 1,
                e.ClipRectangle.Height - 1);

            //Lepsza jakość grafiki
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            //Rysujemy warcaby poprzez dostarczone obrazki
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j].Colour == CheckerColour.Red)
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
                    else if (Board[i, j].Colour == CheckerColour.Black)
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



        }
    }
}