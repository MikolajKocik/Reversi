using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Reversi
{
    internal class ReversiEngine
    {
        public int BoardWidth { get; private set; }
        public int BoardHeight { get; private set; }
        public int CurrentPlayerNumber { get; private set; } = 1;
        private int[,] board;
        private int[] fieldCounts = new int[3];
        public int EmptyFieldCount { get { return fieldCounts[0]; } }
        public int Player1FieldCount { get { return fieldCounts[1]; } }
        public int Player2FieldCount { get { return fieldCounts[2]; } }

        public int LeadingPlayerNumber
        {
            get
            {
                if (Player1FieldCount == Player2FieldCount) return 0;
                else if (Player1FieldCount > Player2FieldCount) return 1;
                else return 2;
            }
        }

        public enum BoardSituation
        {
            MoveIsPossible,
            CurrentPlayerCannotMove,
            BothPlayersCannotMove,
            AllFieldsOccupied
        }

        public ReversiEngine(int startingPlayerNumber, int boardWidth = 8,
            int boardHeight = 8)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            board = new int[boardWidth, boardHeight];
            CurrentPlayerNumber = startingPlayerNumber;
            ClearBoard();
            CalculateFieldCounts();
        }

        private void CalculateFieldCounts()
        {
            fieldCounts[0] = 0;
            fieldCounts[1] = 0;
            fieldCounts[2] = 0;

            for (int i = 0; i < BoardWidth; i++)
            {
                for (int j = 0; j < BoardHeight; j++)   
                {
                    fieldCounts[board[i, j]]++;
                }
            }
        }

        private void SwitchCurrentPlayer()
        {
            CurrentPlayerNumber =
                GetOpponentNumber(CurrentPlayerNumber);
        }

        private void ClearBoard()
        {
            for (int i = 0; i < BoardWidth; i++)
            {
                for (int j = 0; j < BoardHeight; j++)
                {
                    board[i, j] = 0;
                }
            }

            int centerX = BoardWidth / 2;
            int centerY = BoardHeight / 2;
            board[centerX - 1, centerY - 1] = 1;
            board[centerX, centerY] = 1;
            board[centerX, centerY - 1] = 2;
            board[centerX - 1, centerY] = 2;
        }

        public BoardSituation CheckBoardSituation()
        {
            if (EmptyFieldCount == 0) return BoardSituation.AllFieldsOccupied;

            bool canMove = CanCurrentPlayerMove();
            if (canMove) return BoardSituation.MoveIsPossible;
            else
            {
                SwitchCurrentPlayer();
                bool canOpponentMove = CanCurrentPlayerMove();
                SwitchCurrentPlayer();
                if (canOpponentMove)
                    return BoardSituation.CurrentPlayerCannotMove;
                else
                {
                    return BoardSituation.BothPlayersCannotMove;
                }
            }
        }

        public int GetFieldState(int x, int y)
        {
            if (!AreCoordinatesValid(x, y))
                throw new Exception("Invalid field coordinates");

            return board[x, y];
        }

        private bool AreCoordinatesValid(int x, int y)
        {
            return x >= 0 && y >= 0 &&
                x < BoardWidth && y < BoardHeight;
        }

        private static int GetOpponentNumber(int playerNumber)
        {
            if (playerNumber == 1) return 2;
            else return 1;
        }

        private bool CanCurrentPlayerMove()
        {
            int validFieldCount = 0;
            for(int i = 0; i < BoardWidth; i++)
            {
                for(int j = 0; j < BoardHeight; j++)
                {
                    if (board[i, j] == 0 && PlaceStone(i, j, true) > 0)
                    {
                        validFieldCount++;
                    }
                }
            }
            return validFieldCount > 0;
        }

        public void Pass()
        {
            if (CanCurrentPlayerMove())
                throw new Exception("Player cannot pass - a move is possible");
            SwitchCurrentPlayer();
        }

        public int PlaceStone(int x, int y)
        {
            return PlaceStone(x, y, false);
        }

        protected int PlaceStone(int x, int y, bool testOnly)
        {
            if (board[x, y] != 0) return -1;
            int capturedFieldCount = 0;
            for(int directionX = -1; directionX <= 1; directionX++)
                for(int directionY = -1; directionY <= 1; directionY++)
                {
                    if (directionY == 0 && directionX == 0) continue; 
                    int i = x;
                    int j = y;
                    bool foundEmptyField = false;
                    bool reachedBoardEdge = false;
                    bool foundCurrentPlayerStone = false;
                    bool foundOpponentStone = false;
                    do
                    {
                        i += directionX;
                        j += directionY;
                        if (!AreCoordinatesValid(i, j)) reachedBoardEdge = true;
                        if (!reachedBoardEdge)
                        {
                            if (board[i, j] == 0) foundEmptyField = true;
                            if (board[i, j] == CurrentPlayerNumber)
                                foundCurrentPlayerStone = true;
                            if (board[i, j] == GetOpponentNumber(CurrentPlayerNumber))
                                foundOpponentStone = true;
                        }
                    }
                    while (!(foundEmptyField || reachedBoardEdge 
                        || foundCurrentPlayerStone));

                    bool placementPossible = foundOpponentStone &&
                         foundCurrentPlayerStone && !foundEmptyField;

                    if (placementPossible)
                    {
                        int maxIndex = Math.Max(Math.Abs(i - x), Math.Abs(j - y));
                        if (!testOnly)
                        {
                            for (int index = 0; index < maxIndex; index++)
                            {
                                board[x + index * directionX, y + index * directionY] = CurrentPlayerNumber;
                            }
                        }
                        capturedFieldCount += maxIndex - 1;
                    }
                }
            if (capturedFieldCount > 0 && !testOnly) SwitchCurrentPlayer();
            CalculateFieldCounts();
            return capturedFieldCount;
        }
    }
}
