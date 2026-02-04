using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi
{
    internal class ReversiEngineAI : ReversiEngine
    {
        public ReversiEngineAI(int startingPlayerNumber, 
            int boardWidth = 8 , int boardHeight = 8) 
            : base(startingPlayerNumber, boardWidth, boardHeight)
        { }

        private struct PossibleMove : IComparable<PossibleMove>
        {
            public int x;
            public int y;
            public int priority;
            public PossibleMove(int x, int y, int priority)
            {
                this.x = x;
                this.y = y;
                this.priority = priority;
            }

            public int CompareTo(PossibleMove otherMove)
            {
                return otherMove.priority - this.priority;
            }
        }

        public void SuggestBestMove(out int bestMoveX, out int bestMoveY)
        {
            List<PossibleMove> possibleMoves = new List<PossibleMove>();
            int priorityStep = BoardWidth + BoardHeight;
            for(int i = 0; i < BoardWidth; i++)
            {
                for(int j = 0; j < BoardHeight; j++)
                {
                    if (GetFieldState(i, j) == 0)
                    {
                        int priority = PlaceStone(i, j, true);
                        if(priority > 0)
                        {
                            PossibleMove move = new PossibleMove(i, j, priority);
                            if((i == 0 || i == BoardWidth - 1) && (j == 0 || j == BoardHeight - 1))
                            {
                                move.priority += 2 * priorityStep;
                            }
                            if((i == 0 || i == BoardWidth - 1) && (j == 1 || j == BoardHeight - 2))
                            {
                                move.priority -= 2 * priorityStep;
                            }
                            if((i == 1 || i == BoardWidth - 2) && (j == 0 || j == BoardHeight - 1))
                            {
                                move.priority -= 2 * priorityStep;
                            }
                            if((i == 1 || i == BoardWidth - 2) && (j == 1 || j == BoardHeight - 2))
                            {
                                move.priority -= 2 * priorityStep;
                            }
                            if((i == 0 || i == BoardWidth - 1) || (j == 0 || j == BoardHeight - 1))
                            {
                                move.priority += priorityStep;
                            }
                            if((i == 1 || i == BoardWidth - 2) || (j == 1 || j == BoardHeight - 2))
                            {
                                move.priority -= priorityStep;
                            }
                            possibleMoves.Add(move);
                        }
                    }
                }     
            }
            if (possibleMoves.Count > 0)
            {
                possibleMoves.Sort();
                bestMoveX = possibleMoves[0].x;
                bestMoveY = possibleMoves[0].y;
            }
            else
            {
                throw new Exception("No possible moves");
            }
        }
    }
}
