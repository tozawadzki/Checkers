using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AICheckers
{
    class MonteCarlo : IAI
    {
        private static double CHANCE = 0.5;
        private static int checks = 1000;
        private static int DEPTH = 150;
        private static Random random = new Random();
        CheckerColor color;

        public CheckerColor Color
        {
            get { return color; }
            set { color = value; }
        }

        public Move Process(Square[,] Board)
        {
            return getAIMove(DEPTH, DeepCopy(Board));
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

        public static Square[,] ExecuteVirtualMove(Move move, Square[,] Board)
        {
            Board[move.Destination.Y, move.Destination.X].Color = Board[move.Source.Y, move.Source.X].Color;
            Board[move.Destination.Y, move.Destination.X].King = Board[move.Source.Y, move.Source.X].King;
            Board[move.Source.Y, move.Source.X].Color = CheckerColor.Empty;
            Board[move.Source.Y, move.Source.X].King = false;

            if ((move.Destination.Y == Constants.BOARD_SIZE - 1 && Board[move.Destination.Y, move.Destination.X].Color == CheckerColor.Red)
                || (move.Destination.Y == 0 && Board[move.Destination.Y, move.Destination.X].Color == CheckerColor.Black))
            {
                Board[move.Destination.Y, move.Destination.X].King = true;
            }

            return Board;
        }

        public Move getAIMove(int depth, Square[,] Board)
        {
            List<Move> moves = getAllMoves(Board, Color);
            List<TreeNode> nodes = new List<TreeNode>(moves.Count());
            int i = 0;

            foreach (Move move in moves)
            {
                TreeNode node = new TreeNode(move, Board, i++, Color);
                nodes.Add(node);
            }

            nodes.Sort();
            checks = 2000;

            for (i = 0; i < checks; i++)
            {
                double picked = random.NextDouble();
                double currentChance = CHANCE;
                for (int j = 0; j < nodes.Count(); j++)
                {
                    if (picked >= currentChance || j == nodes.Count() - 1)
                    {
                        nodes[j].visit();
                        break;
                    }
                    else
                        currentChance *= CHANCE;
                }
                if (i % 10 == 9)
                    nodes.Sort((o1, o2)=>((o1.wins - o1.losses) / (float)o1.total).CompareTo((o2.wins - o2.losses) / (float)o2.total));
            }
            float max = float.NegativeInfinity;
            TreeNode maxNode = null;

            foreach (TreeNode node in nodes)
            {
                if (node.total < 30)
                    continue;
                float score = (node.wins - node.losses) / (float)node.total;
                if (score >= max)
                {
                    maxNode = node;
                    max = score;
                }
            }
            if (maxNode == null)
            {
                Environment.Exit(0);
            }
            return maxNode.move;
        }

        public static CheckerColor playRandomGame(Square[,] Board, CheckerColor color)
        {
            List<Move> moves = getAllMoves(Board, color);

            moves.Sort((a, b)=> {
                if (a.isJump && !b.isJump)
                    return -1;
                if (b.isJump && !a.isJump)
                    return 1;
                return 0;
            });

            int depth = DEPTH;

            while (moves.Count() != 0 && depth != 0)
            {
                Square[,] vBoard = ExecuteVirtualMove(moves[random.Next(moves.Count())], Board);
                moves = getAllMoves(vBoard, color);
                depth--;
            }
            var redCheckers = Board.Cast<Square>().Where(x => x.Color == CheckerColor.Red);
            var blackCheckers = Board.Cast<Square>().Where(x => x.Color == CheckerColor.Black);

            if (moves.Count() == 0)
                return CheckerColor.Red == color ? CheckerColor.Black : CheckerColor.Red;
            else
            {
                var currentScore = Board.Cast<Square>().Where(x => x.Color == color).Count();

                if (currentScore > 2)
                    return color;
                if (currentScore < -2)
                    return CheckerColor.Red == color ? CheckerColor.Black : CheckerColor.Red;

                return CheckerColor.Empty;
            }
        }

        private static float getScore(Square[,] board, int depth, Boolean maxing, float alpha, float beta, CheckerColor color)
        {
            List<Move> moves = getAllMoves(board, color);
            var currentScore = board.Cast<Square>().Where(x => x.Color == color).Count();

            if (depth == 0)
                return maxing ? currentScore : -currentScore;
            if (moves.Count() == 0)
                return maxing ? float.NegativeInfinity : float.PositiveInfinity;
            if (maxing)
            {
                float best = float.NegativeInfinity;

                foreach (Move move in moves)
                {
                    Square[,] vBoard = ExecuteVirtualMove(move, board);
                    float score = getScore(vBoard, depth - 1, false, alpha, beta, color);
                    best = Math.Max(best, score);
                    alpha = Math.Max(alpha, score);

                    if (beta <= alpha)
                        break;
                }

                return best;
            }
            else    //minimising
            {
                float best = float.PositiveInfinity;

                foreach (Move move in moves)
                {
                    Square[,] vBoard = ExecuteVirtualMove(move, board);
                    float score = getScore(vBoard, depth - 1, false, alpha, beta, color);
                    best = Math.Min(best, score);
                    beta = Math.Min(beta, score);

                    if (beta <= alpha)
                        break;
                }

                return best;
            }
        }

        public static List<Move> getAllMoves(Square[,] Board, CheckerColor color)
        {
           List<Move> moves = new List<Move>();

            for (int i = 0; i < Constants.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Constants.BOARD_SIZE; j++)
                {
                    if (Board[i, j].Color == color)
                    {
                        foreach (Move myPossibleMove in Minimax.GetOpenSquares(Board, new Point(j, i)))
                        {
                            moves.Add(myPossibleMove);
                        }
                    }
                }
            }

            return moves;
        }

        private class TreeNode : IComparable
        {
            public int wins = 0;
            public int losses = 0;
            public int total = 0;
            public Move move;
            public float score;
            public int index;
            public Square[,] board;
            public CheckerColor me;

            public TreeNode(Move move, Square[,] board, int index, CheckerColor color)
            {
                this.move = move;
                this.me = color;
                this.board = board;
                this.score = getScore(board, 5, false, float.NegativeInfinity, float.PositiveInfinity, color);
                this.index = index;
            }

            public void visit()
            {
                CheckerColor winner = playRandomGame(board, me);

                if (winner == me)
                    wins++;
                else
                    losses++;

                total++;
            }

            public int CompareTo(object obj)
            {
                TreeNode other = obj as TreeNode;
                if (this == other)
                    return 0;

                else
                {
                    int compare = score.CompareTo(other.score);

                    if (compare == 0)
                        return other.index - index;

                    else
                        return compare;
                }
            }
        }
    }
}
