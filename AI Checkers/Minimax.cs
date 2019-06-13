using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AICheckers
{
    /// <summary>
    /// MINIMAX
    /// </summary>
    static class Minimax
    {
        /// <summary>
        /// "Konstruktor" ruchu
        /// Domyślny ruch
        /// </summary>
        /// <param name="Board"></param>
        /// <param name="checker"></param>
        /// <returns></returns>
        public static Move[] GetOpenSquares(Square[,] Board, Point checker)
        {
            return GetOpenSquares(Board, checker, new Move(-1, -1, -1, -1), null);
        }

        /// <summary>
        /// Metoda odpowiadająca za ruchy warcabem, w zależności od położenia warcaba
        /// </summary>
        /// <param name="Board"></param>
        /// <param name="checker"></param>
        /// <param name="lastMove"></param>
        /// <param name="priorPositions"></param>
        /// <returns></returns>
        private static Move[] GetOpenSquares(Square[,] Board, Point checker, Move lastMove, List<Point> priorPositions)
        {
            if (priorPositions == null)
            {
                priorPositions = new List<Point>();
                priorPositions.Add(checker);
            }

            List<Move> OpenSquares = new List<Move>();

            //KRÓL: RUCH W GÓRA LEWO
            if (Board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Red || Board[priorPositions[0].Y, priorPositions[0].X].King)       
                // W warunku znajduje się state, że zwykły pionek nie może się ruszyć w górę
            {
                if (IsValidPoint(checker.X - 1, checker.Y - 1))
                {
                    if (Board[checker.Y - 1, checker.X - 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)
                    {
                        OpenSquares.Add(new Move(priorPositions[0], checker.X - 1, checker.Y - 1));
                    }
                    //Sprawdzenie, czy pole jest accessible
                    else if (IsValidPoint(checker.X - 2, checker.Y - 2)
                        && ((checker.X - 2) != lastMove.Destination.X || (checker.Y - 2) != lastMove.Destination.Y)
                        && ((checker.X - 2) != priorPositions[0].X || (checker.Y - 2) != priorPositions[0].Y)
                        && Board[checker.Y - 1, checker.X - 1].Colour != Board[checker.Y, checker.X].Colour
                        && Board[checker.Y - 2, checker.X - 2].Colour == CheckerColour.Empty)
                    {
                        //Stworzenie nowego punktu, w którym znajdzie się warcab
                        Point newDest = new Point(checker.X - 2, checker.Y - 2);
                        //Jeśli dana pozycja nie jest zajęta
                        if (!priorPositions.Contains(newDest))
                        {
                            // Odbywa się ruch
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X - 1, checker.Y - 1));
                            move.Captures.AddRange(lastMove.Captures);
                            OpenSquares.Add(move);

                            priorPositions.Add(newDest);

                            //Rekurencja, jeśli jest możliwy ruch o więcej niż 1 pole (zbicie warcaba)           
                            OpenSquares.AddRange(GetOpenSquares(Board, new Point(checker.X - 2, checker.Y - 2), move, priorPositions));
                        }
                    }
                }
            }

            //KRÓL: RUCH GÓRA PRAWO
            if (Board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Red || Board[priorPositions[0].Y, priorPositions[0].X].King)
            {
                if (IsValidPoint(checker.X + 1, checker.Y - 1))
                {
                    if (Board[checker.Y - 1, checker.X + 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)
                    {
                        OpenSquares.Add(new Move(priorPositions[0], checker.X + 1, checker.Y - 1));
                    }
                    else if (IsValidPoint(checker.X + 2, checker.Y - 2)
                        && ((checker.X + 2) != lastMove.Destination.X || (checker.Y - 2) != lastMove.Destination.Y)
                        && ((checker.X + 2) != priorPositions[0].X || (checker.Y - 2) != priorPositions[0].Y)
                        && Board[checker.Y - 1, checker.X + 1].Colour != Board[checker.Y, checker.X].Colour
                        && Board[checker.Y - 2, checker.X + 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X + 2, checker.Y - 2);
                        if (!priorPositions.Contains(new Point(checker.X + 2, checker.Y - 2)))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X + 1, checker.Y - 1));
                            move.Captures.AddRange(lastMove.Captures);
                            OpenSquares.Add(move);

                            priorPositions.Add(newDest);

                            OpenSquares.AddRange(GetOpenSquares(Board, new Point(checker.X + 2, checker.Y - 2), move, priorPositions));
                        }
                    }
                }
            }

            //RUCH W DÓŁ LEWO
            if (Board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Black || Board[priorPositions[0].Y, priorPositions[0].X].King)
            {
                if (IsValidPoint(checker.X - 1, checker.Y + 1))
                {
                    if (Board[checker.Y + 1, checker.X - 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)
                    {
                        OpenSquares.Add(new Move(priorPositions[0], checker.X - 1, checker.Y + 1));
                    }
                    else if (IsValidPoint(checker.X - 2, checker.Y + 2)
                        && ((checker.X - 2) != lastMove.Destination.X || (checker.Y + 2) != lastMove.Destination.Y)
                        && ((checker.X - 2) != priorPositions[0].X || (checker.Y + 2) != priorPositions[0].Y)
                        && Board[checker.Y + 1, checker.X - 1].Colour != Board[checker.Y, checker.X].Colour
                        && Board[checker.Y + 2, checker.X - 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X - 2, checker.Y + 2);
                        if (!priorPositions.Contains(newDest))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X - 1, checker.Y + 1));
                            move.Captures.AddRange(lastMove.Captures);
                            OpenSquares.Add(move);

                            priorPositions.Add(newDest);

                            OpenSquares.AddRange(GetOpenSquares(Board, new Point(checker.X - 2, checker.Y + 2), move, priorPositions));
                        }
                    }
                }
            }

            //RUCH W DÓŁ PRAWO
            if (Board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Black || Board[priorPositions[0].Y, priorPositions[0].X].King)
            {
                if (IsValidPoint(checker.X + 1, checker.Y + 1))
                {
                    if (Board[checker.Y + 1, checker.X + 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)
                    {
                        OpenSquares.Add(new Move(priorPositions[0], checker.X + 1, checker.Y + 1));
                    }
                    else if (IsValidPoint(checker.X + 2, checker.Y + 2)
                        && ((checker.X + 2) != lastMove.Destination.X || (checker.Y + 2) != lastMove.Destination.Y)
                        && ((checker.X + 2) != priorPositions[0].X || (checker.Y + 2) != priorPositions[0].Y)
                        && Board[checker.Y + 1, checker.X + 1].Colour != Board[checker.Y, checker.X].Colour
                        && Board[checker.Y + 2, checker.X + 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X + 2, checker.Y + 2);
                        if (!priorPositions.Contains(newDest))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X + 1, checker.Y + 1));
                            move.Captures.AddRange(lastMove.Captures);
                            OpenSquares.Add(move);

                            priorPositions.Add(newDest);

                            OpenSquares.AddRange(GetOpenSquares(Board, new Point(checker.X + 2, checker.Y + 2), move, priorPositions));
                        }
                    }
                }
            }

            return OpenSquares.ToArray();
        }

        /// <summary>
        /// Metoda sprawdzająca, czy ruch nie wykracza poza planszę
        /// jej argumenty to położenie x i y na planszy
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool IsValidPoint(int x, int y)
        {
            if (0 <= x && x < 8 && 0 <= y && y < 8) return true;
            return false;
        }

        /// <summary>
        /// JW
        /// argumenty to zmienna typu Point, również posiadająca x i y
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private static bool IsValidPoint(Point point)
        {
            return (IsValidPoint(point.X, point.Y));
        }
    }
}
