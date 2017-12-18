using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Sudoku
{
    public class Solver
    {
        private Board _currentBoard;

        public Solver(Board board)
        {
            _currentBoard = board;
        }

        /// <summary>
        /// Tries to solve the board
        /// </summary>
        /// <returns>A new instance of Board if solved otherwise null</returns>
        public Board Solve()
        {
            List<int>[,] possibleValues = FindValidValues(_currentBoard);

            Board copiedBoard = _currentBoard.Copy();
            _currentBoard = copiedBoard;
            if (Solve(possibleValues))
            {
                return copiedBoard;
            }

            return null;
        }

        private bool Solve(List<int>[,] possibleValues)
        {
            if (FindBestCellToFill(possibleValues, out Point bestCell))
            {
                List<int> values = possibleValues[bestCell.Y, bestCell.X];

                foreach (int value in values)
                {
                    _currentBoard.Set(bestCell, value);
                    if (!_currentBoard.HasError(bestCell))
                    {
                        if (Solve(possibleValues))
                        {
                            return true;
                        }
                    }

                    _currentBoard.Set(bestCell, 0);
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the first cell that has the lowest amount of options available
        /// </summary>
        /// <param name="possibleValues"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool FindBestCellToFill(List<int>[,] possibleValues, out Point point)
        {
            Point best = new Point(-1, -1);
            int bestAmount = 0;

            for (int x = 0; x < Board.Size; x++)
            {
                for (int y = 0; y < Board.Size; y++)
                {
                    if(_currentBoard.Get(new Point(x,y)) != 0)
                        continue;

                    if (possibleValues[y, x].Count == 1)
                    {
                        point = new Point(x, y);
                        return true;
                    }

                    if (bestAmount == 0 || possibleValues[y, x].Count < bestAmount)
                    {
                        if (possibleValues[y, x].Count == 0)
                            continue;

                        best.X = x;
                        best.Y = y;
                        bestAmount = possibleValues[y, x].Count;
                    }
                }
            }

            if (bestAmount == 0)
            {
                point = new Point();
                return false;
            }

            point = best;
            return true;
        }

        private List<int>[,] FindValidValues(Board board)
        {
            List<int>[,] possibleValues = new List<int>[Board.Size, Board.Size];

            Fill2DArrayWithRange(possibleValues, 1, Board.Size);

            for (int x = 0; x < Board.Size; x++)
            {
                for (int y = 0; y < Board.Size; y++)
                {
                    int value = board.Get(new Point(x, y));
                    if (value != 0)
                    {
                        possibleValues[y, x].Clear();
                        RemoveValueFromRow(possibleValues, y, value);
                        RemoveValueFromCollumn(possibleValues, x, value);
                        RemoveValueFromArea(possibleValues, board.GetAreaStartPointFromPointOnBoard(new Point(x, y)), value);
                    }
                }
            }

            return possibleValues;
        }

        private static void Fill2DArrayWithRange(List<int>[,] possibleValues, int start, int count)
        {
            for (int x = 0; x < Board.Size; x++)
            {
                for (int y = 0; y < Board.Size; y++)
                {
                    possibleValues[y, x] = Enumerable.Range(start, count).ToList();
                }
            }
        }

        private void RemoveValueFromRow(List<int>[,] list, int row, int value)
        {
            for (int i = 0; i < Board.Size; i++)
            {
                list[row, i].Remove(value);
            }
        }

        private void RemoveValueFromCollumn(List<int>[,] list, int collumn, int value)
        {
            for (int i = 0; i < Board.Size; i++)
            {
                list[i, collumn].Remove(value);
            }
        }

        private void RemoveValueFromArea(List<int>[,] list, Point area, int value)
        {
            for (int x = area.X; x < area.X + Board.AreaSize; x++)
            {
                for (int y = area.Y; y < area.Y + Board.AreaSize; y++)
                {
                    list[y, x].Remove(value);
                }
            }
        }
    }
}