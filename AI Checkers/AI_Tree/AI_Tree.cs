﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AICheckers
{
    class AI_Tree : IAI
    {
        //Maksymalne "zagięcie", określamy w każdym drzewie behawioralnym
        readonly int AI_MAXPLYLEVEL = 2;

        //Waga ruchów ofensywnych
        readonly int WEIGHT_CAPTUREPIECE = 2;
        readonly int WEIGHT_CAPTUREKING = 1;
        readonly int WEIGHT_CAPTUREDOUBLE = 5;
        readonly int WEIGHT_CAPTUREMULTI = 10;

        //Waga ruchów obronnych
        readonly int WEIGHT_ATRISK = 3;
        readonly int WEIGHT_KINGATRISK = 4;
        
        CheckerColour colour;

        Tree<Move> gameTree;

        public CheckerColour Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        public Move Process(Square[,] Board)
        {
          
            gameTree = new Tree<Move>(new Move());

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j].Colour == Colour)
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
            Square[,] result = new Square[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    result[i, j] = new Square();
                    result[i, j].Colour = sourceBoard[i, j].Colour;
                    result[i, j].King = sourceBoard[i, j].King;
                }
            }

            return result;
        }

        private void CalculateChildMoves(int recursionLevel, Tree<Move> branch, Move move, Square[,] vBoard)
        {
            if (recursionLevel >= AI_MAXPLYLEVEL)
            {
                return;
            }

            CheckerColour moveColour = vBoard[move.Source.Y, move.Source.X].Colour;

            vBoard = ExecuteVirtualMove(move, vBoard);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (vBoard[i, j].Colour != moveColour)
                    {
                        foreach (Move otherPlayerMove in Minimax.GetOpenSquares(vBoard, new Point(j, i)))
                        {
                            if (vBoard[i, j].Colour != CheckerColour.Empty)
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
            Board[move.Destination.Y, move.Destination.X].Colour = Board[move.Source.Y, move.Source.X].Colour;
            Board[move.Destination.Y, move.Destination.X].King = Board[move.Source.Y, move.Source.X].King;
            Board[move.Source.Y, move.Source.X].Colour = CheckerColour.Empty;
            Board[move.Source.Y, move.Source.X].King = false;

            if ((move.Destination.Y == 7 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Red)
                || (move.Destination.Y == 0 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Black))
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

            score += move.Captures.Count * WEIGHT_CAPTUREPIECE;

            if (move.Captures.Count == 2) score += WEIGHT_CAPTUREDOUBLE;
            if (move.Captures.Count > 2) score += WEIGHT_CAPTUREMULTI;

            foreach (Point point in move.Captures)
            {
                if (board[point.Y, point.X].King) score += WEIGHT_CAPTUREKING;
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].Colour == Colour)
                    {
                        foreach (Move opponentMove in Minimax.GetOpenSquares(board, new Point(j, i)))
                        {
                            if (opponentMove.Captures.Contains(move.Source))
                            {
                                if (board[move.Source.Y, move.Source.X].King)
                                {
                                    score += WEIGHT_KINGATRISK;
                                }
                                else
                                {
                                    score += WEIGHT_ATRISK;
                                }
                            }
                        }
                    }
                }
            }
            if (board[move.Source.Y, move.Source.X].Colour != colour) score *= -1;

            return score;
        }

    }
}
