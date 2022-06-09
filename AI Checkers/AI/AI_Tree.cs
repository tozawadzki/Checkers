using System;
using System.Linq;
using System.Drawing;

namespace AICheckers
{
    class AI_Tree : IAI
    {
        
        CheckerColor color;

        Tree<Move> gameTree;

        public CheckerColor Color
        {
            get { return color; }
            set { color = value; }
        }

        public Move Process(Square[,] Board)
        {
          
            gameTree = new Tree<Move>(new Move());

            for (int i = 0; i < Constants.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Constants.BOARD_SIZE; j++)
                {
                    if (Board[i, j].Color == Color)
                    {
                        foreach (Move myPossibleMove in Minimax.GetOpenSquares(Board, new Point(j, i)))
                        {                    
                            CalculateChildMoves(0, gameTree.AddChild(myPossibleMove), myPossibleMove, DeepCopy(Board));
                        }
                    }
                }
            }

            ScoreTreeMoves(Board);

            return SumTreeMoves();
        }

        private Square[,] DeepCopy(Square[,] sourceBoard)
        {
            Square[,] result = new Square[Constants.BOARD_SIZE, Constants.BOARD_SIZE];

            for (int i = 0; i < Constants.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Constants.BOARD_SIZE; j++)
                {
                    result[i, j] = new Square();
                    result[i, j].Color = sourceBoard[i, j].Color;
                    result[i, j].King = sourceBoard[i, j].King;
                }
            }

            return result;
        }

        private void CalculateChildMoves(int recursionLevel, Tree<Move> branch, Move move, Square[,] vBoard)
        {
            if (recursionLevel >= Constants.AI_MAXPLYLEVEL)
            {
                return;
            }

            CheckerColor moveColor = vBoard[move.Source.Y, move.Source.X].Color;

            vBoard = ExecuteVirtualMove(move, vBoard);

            for (int i = 0; i < Constants.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Constants.BOARD_SIZE; j++)
                {
                    if (vBoard[i, j].Color != moveColor)
                    {
                        foreach (Move otherPlayerMove in Minimax.GetOpenSquares(vBoard, new Point(j, i)))
                        {
                            if (vBoard[i, j].Color != CheckerColor.Empty)
                            {
                                CalculateChildMoves(
                                    ++recursionLevel,
                                    branch.AddChild(otherPlayerMove),
                                    otherPlayerMove,
                                    DeepCopy(vBoard));
                            }
                        }
                    }
                }
            }
        }

        private Square[,] ExecuteVirtualMove(Move move, Square[,] Board)
        {
            Board[move.Destination.Y, move.Destination.X].Color = Board[move.Source.Y, move.Source.X].Color;
            Board[move.Destination.Y, move.Destination.X].King = Board[move.Source.Y, move.Source.X].King;
            Board[move.Source.Y, move.Source.X].Color = CheckerColor.Empty;
            Board[move.Source.Y, move.Source.X].King = false;

            if ((move.Destination.Y == Constants.BOARD_SIZE-1 && Board[move.Destination.Y, move.Destination.X].Color == CheckerColor.Red)
                || (move.Destination.Y == 0 && Board[move.Destination.Y, move.Destination.X].Color == CheckerColor.Black))
            {
                Board[move.Destination.Y, move.Destination.X].King = true;
            }

            return Board;
        }

        private void ScoreTreeMoves(Square[,] Board)
        {
            Action<Move> scoreMove = (Move move) => move.Score = ScoreMove(move, Board);

            foreach (Tree<Move> possibleMove in gameTree.Children)
            {
                possibleMove.Traverse(scoreMove);
            }

        }

        private Move SumTreeMoves()
        {
            int branchSum = 0;
            Action<Move> sumScores = (Move move) => branchSum += move.Score;

            foreach (Tree<Move> possibleMove in gameTree.Children)
            {
                possibleMove.Traverse(sumScores);
                possibleMove.Value.Score += branchSum;
                branchSum = 0;
            }

            return gameTree.Children.OrderByDescending(o => o.Value.Score).ToList()[0].Value;
        }

        private int ScoreMove(Move move, Square[,] board)
        {
            int score = 0;

            score += move.Captures.Count * Constants.WEIGHT_CAPTUREPIECE;

            if (move.Captures.Count == 2) score += Constants.WEIGHT_CAPTUREDOUBLE;
            if (move.Captures.Count > 2) score += Constants.WEIGHT_CAPTUREMULTI;

            foreach (Point point in move.Captures)
            {
                if (board[point.Y, point.X].King) score += Constants.WEIGHT_CAPTUREKING;
            }

            for (int i = 0; i < Constants.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Constants.BOARD_SIZE; j++)
                {
                    if (board[i, j].Color == Color)
                    {
                        foreach (Move opponentMove in Minimax.GetOpenSquares(board, new Point(j, i)))
                        {
                            if (opponentMove.Captures.Contains(move.Source))
                            {
                                if (board[move.Source.Y, move.Source.X].King)
                                {
                                    score += Constants.WEIGHT_KINGATRISK;
                                }
                                else
                                {
                                    score += Constants.WEIGHT_ATRISK;
                                }
                            }
                        }
                    }
                }
            }
            if (board[move.Source.Y, move.Source.X].Color != color) score *= -1;

            return score;
        }

    }
}
