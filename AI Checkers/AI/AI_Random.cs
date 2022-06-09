using System;
using System.Collections.Generic;
using System.Drawing;

namespace AICheckers
{
    class AI_Random : IAI
    {
        CheckerColor color;

        public CheckerColor Color
        {
            get { return color; }
            set { color = value; }
        }

        public Move Process(Square[,] Board)
        {
            List<Move> moves = new List<Move>();

            for (int i = 0; i < Constants.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Constants.BOARD_SIZE; j++)
                {
                    if (Board[i, j].Color == Color)
                    {
                        moves.AddRange(Minimax.GetOpenSquares(Board, new Point(j, i)));
                    }
                }
            }

            return moves[(new Random()).Next(moves.Count)];
        }
    }
}
