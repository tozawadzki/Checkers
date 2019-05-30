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
        Color darkSquare = Color.DarkGray;
        Color lightSquare = Color.Gainsboro;

        //Upload obrazków 
        Image checkerRed = Resources.checkerred;
        Image checkerRedKing = Resources.checkerredking;
        Image checkerBlack = Resources.checkerblack;
        Image checkerBlackKing = Resources.checkerblackking;

        //Animacje
        private bool animating = false;
        const int animDuration = 1000;
        private Square animPiece;

        //Ustawienie domyślnych wartości dla punktów
        private Point oldPoint = new Point(-1, -1);
        private Point currentPoint = new Point(-1, -1);
        private Point newPoint = new Point(-1, -1);

        int squareWidth = 0;

        //Pola do operowania warcabami
        private Point selectedChecker = new Point(-1, -1);
        private List<Move> possibleMoves = new List<Move>();

        //Określenie tury 
        CheckerColour currentTurn = CheckerColour.Black;
        private System.ComponentModel.IContainer components;

        private Square[,] Board = new Square[8,8];
        
        //Inicjalizacja panelu
        public BoardPanel()
            : base()
        {
            //this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint, true);

            //Inicjalizacja planszy
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Board[i, j] = new Square();
                    Board[i, j].Colour = CheckerColour.Empty;
                }
            }
            
            //Przygotowanie warcabów (rozmieszczenie na planszy czarnych i czerwonych)
            for (int i = 0; i < 8; i += 1)
            {
                int offset = 0;
                if (i % 2 != 0)
                {
                    offset++;
                }
                for (int j = offset; j < 8; j += 2)
                {
                    if (i < 3) Board[i, j].Colour = CheckerColour.Red;
                    if (i > 4) Board[i, j].Colour = CheckerColour.Black;
                }
            }

            AI = new AI_Tree();
            AI.Colour = CheckerColour.Black;

            AdvanceTurn();
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            //Rysowanie kwadracików
            base.OnPaint(e);                        
            e.Graphics.Clear(lightSquare);

            //Rysowanie planszy
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
                e.Graphics.FillRectangle(Brushes.PaleTurquoise, move.Destination.X * squareWidth, move.Destination.Y * squareWidth, squareWidth, squareWidth);
            }

            //Po wybraniu warcaba
            if (selectedChecker.X >= 0 && selectedChecker.Y >= 0)
            {
                e.Graphics.FillRectangle(Brushes.PeachPuff, selectedChecker.X * squareWidth, selectedChecker.Y * squareWidth, squareWidth, squareWidth);
            }

            //Narysowanie granicy
            e.Graphics.DrawRectangle(Pens.DarkGray,
            e.ClipRectangle.Left,
            e.ClipRectangle.Top,
            e.ClipRectangle.Width - 1,
            e.ClipRectangle.Height - 1);

            //W celu fajniejszego konwertowania obrazków
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            //Pobranie obrazków w celu przedstawienia ich na planszy
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i,j].Colour == CheckerColour.Red)
                    {
                        if (Board[i, j].King)
                        {
                            e.Graphics.DrawImage(checkerRedKing, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                        else
                        {
                            e.Graphics.DrawImage(checkerRed, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                    }
                    else if (Board[i, j].Colour == CheckerColour.Black)
                    {
                        if (Board[i, j].King)
                        {
                            e.Graphics.DrawImage(checkerBlackKing, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                        else
                        {
                            e.Graphics.DrawImage(checkerBlack, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Metoda wykonująca się po kliknięciu myszy
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            int clickedX = (int)(((double)e.X / (double)Width) * 8.0d);
            int clickedY = (int)(((double)e.Y / (double)Height) * 8.0d);

            Point clickedPoint = new Point(clickedX, clickedY);

            
            if (Board[clickedY, clickedX].Colour != CheckerColour.Empty
                && Board[clickedY, clickedX].Colour != currentTurn)
                return;
          
            //Sprawdzamy, czy gracz się porusza czy tylko wybiera warcaba
            List<Move> matches = possibleMoves.Where(m => m.Destination == clickedPoint).ToList<Move>();
            if (matches.Count > 0)
            {
                MoveChecker(matches[0]);
            }
            else if (Board[clickedY, clickedX].Colour != CheckerColour.Empty)
            {
                //Po kliknięciu w warcaba wchodzimy w tryb wybierania i pokazujemy możliwe ruchy
                selectedChecker.X = clickedX;
                selectedChecker.Y = clickedY;
                possibleMoves.Clear();
                
                //Pokazujemy nową planszę po ruchu
                Move[] OpenSquares = Utils.GetOpenSquares(Board, selectedChecker);
                possibleMoves.AddRange(OpenSquares);

                this.Invalidate();
            }            
        }

        /// <summary>
        /// Metoda odpowiedzialna za poruszanie warcabem
        /// Jako argument przyjmuje ruch, jaki wykonał gracz
        /// </summary>
        /// <param name="move"></param>
        private void MoveChecker(Move move)
        {
            Board[move.Destination.Y, move.Destination.X].Colour = Board[move.Source.Y, move.Source.X].Colour;
            Board[move.Destination.Y, move.Destination.X].King = Board[move.Source.Y, move.Source.X].King;
            ResetSquare(move.Source);

            // Odświeżamy kwadraciki
            foreach (Point point in move.Captures)
                ResetSquare(point);

            //"Odznaczenie" warcaba
            selectedChecker.X = -1;
            selectedChecker.Y = -1;

            //Zamiana warcaba w króla
            if ((move.Destination.Y == 7 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Red)
                || (move.Destination.Y == 0 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Black))
            {
                Board[move.Destination.Y, move.Destination.X].King = true;
            }

            //Czyścimy bufor z możliwymi ruchami, aby nie pokazywały się przy kolejnej turze stare możliwości
            possibleMoves.Clear();

            // Ruch
            oldPoint.X = move.Source.Y * squareWidth;
            oldPoint.Y = move.Source.X * squareWidth;
            newPoint.X = move.Destination.Y * squareWidth;
            newPoint.Y = move.Destination.X * squareWidth;
            currentPoint = oldPoint;
            animating = true;

            this.Invalidate();

            //Zmiana tury
            AdvanceTurn();
        }

        /// <summary>
        /// Czyszczenie kwadracików
        /// </summary>
        /// <param name="square"></param>
        private void ResetSquare(Point square)
        {
            //Reset the square and the selected checker
            Board[square.Y, square.X].Colour = CheckerColour.Empty;
            Board[square.Y, square.X].King = false;
        }

        /// <summary>
        /// Metoda rozstrzygająca kto rozgrywa turę
        /// </summary>
        private void AdvanceTurn()
        {
            if (currentTurn == CheckerColour.Red)
            {
                currentTurn = CheckerColour.Black;
            }
            else
            {
                currentTurn = CheckerColour.Red;
            }

            if (AI != null && AI.Colour == currentTurn)
            {
                Move aiMove = AI.Process(Board);
                //TUTAJ RUSZA SIĘ KOMPUTER
                MoveChecker(aiMove);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
