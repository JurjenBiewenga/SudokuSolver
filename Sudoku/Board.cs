using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sudoku
{
    public class Board
    {
        public const int Size = 9;
        public const int AreaSize = 3;

        private int[,] _board = new int[Size, Size];

        public Board()
        {
        }

        public Board(int[,] board)
        {
            this._board = board;
        }

        public int Get(Point point)
        {
            if (point.Y < _board.GetLength(0) && point.X < _board.GetLength(1))
            {
                return _board[point.Y, point.X];
            }

            return 0;
        }

        public void Set(Point point, int value)
        {
            if (point.Y < _board.GetLength(0) && point.X < _board.GetLength(1))
            {
                _board[point.Y, point.X] = value;
            }
        }

        public void Set(int[,] data)
        {
            if (data.GetLength(0) != Size || data.GetLength(1) != Size)
                return;

            _board = data;
        }

        public Board Solve()
        {
            Solver solver = new Solver(this);
            return solver.Solve();
        }

        /// <summary>
        /// Rounds a point on the board to the start position of the area it is in based on <see cref="Board.AreaSize" />, e.g. Point(3,3) becomes Point(1,1) with an AreaSize of 3
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point GetAreaStartPointFromPointOnBoard(Point point)
        {
            return new Point((int)Math.Floor(point.X / (float)AreaSize) * AreaSize, (int)Math.Floor(point.Y / (float)AreaSize) * AreaSize);
        }

        /// <summary>
        /// Converts 1d index to a 2d index for an array of AreaSize, e.g. 4 becomes Point(1, 1)
        /// </summary>
        /// <param name="i">Index between 0 and <see cref="Board.Size" /> </param>
        /// <returns></returns>
        public Point GetAreaIndexFromInt(int i)
        {
            return new Point(i % AreaSize, (int)Math.Floor(i / (float)AreaSize));
        }

        /// <summary>
        /// Converts the 2d index to a point which can be used on the board, e.g. Point(1,1) becomes Point(3,3) based on an AreaSize of 3
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point GetAreaStartPointFrom2DAreaIndex(Point point)
        {
            return new Point(point.X * AreaSize, point.Y * AreaSize);
        }

        public bool HasError()
        {
            for (int i = 0; i < Size; i++)
            {
                bool errorInRow = HasErrorInRow(i);
                if (errorInRow)
                    return true;

                bool errorInCollumn = HasErrorInCollumn(i);
                if (errorInCollumn)
                    return true;

                bool errorInArea = HasErrorInArea(GetAreaStartPointFrom2DAreaIndex(GetAreaIndexFromInt(i)));
                if (errorInArea)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for an error in the row/collumn and area of the point 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool HasError(Point p)
        {
            bool errorInRow = HasErrorInRow(p.Y);
            if (errorInRow)
                return true;

            bool errorInCollumn = HasErrorInCollumn(p.X);
            if (errorInCollumn)
                return true;

            bool errorInArea = HasErrorInArea(GetAreaStartPointFromPointOnBoard(p));
            if (errorInArea)
                return true;


            return false;
        }

        public bool HasErrorInRow(int row)
        {
            List<int> valuesInRow = new List<int>(Size);
            for (int i = 0; i < Size; i++)
            {
                int value = _board[row, i];
                if (value != 0)
                {
                    if (valuesInRow.Contains(value))
                        return true;

                    valuesInRow.Add(value);
                }
            }

            return false;
        }

        public bool HasErrorInCollumn(int collumn)
        {
            List<int> valuesInCollumn = new List<int>(Size);
            for (int i = 0; i < Size; i++)
            {
                int value = _board[i, collumn];
                if (value != 0)
                {
                    if (valuesInCollumn.Contains(value))
                        return true;

                    valuesInCollumn.Add(value);
                }
            }

            return false;
        }

        public bool HasErrorInArea(Point point)
        {
            List<int> valuesInArea = new List<int>(Size);

            for (int x = point.X; x < point.X + AreaSize; x++)
            {
                for (int y = point.Y; y < point.Y + AreaSize; y++)
                {
                    int value = _board[y, x];
                    if (value != 0)
                    {
                        if (valuesInArea.Contains(value))
                            return true;

                        valuesInArea.Add(value);
                    }
                }
            }

            return false;
        }

        public Board Copy()
        {
            int[,] copiedArray = (int[,])_board.Clone();
            return new Board(copiedArray);
        }

        public override string ToString()
        {
            string output = "";
            for (int y = 0; y < Size; y++)
            {
                output += Environment.NewLine;
                if (y % 3 == 0 && y != 0)
                    output += Environment.NewLine;

                for (int x = 0; x < Size; x++)
                {
                    output += " ";
                    if (x % 3 == 0 && x != 0)
                        output += " ";

                    output += _board[y, x];
                }
            }

            return output;
        }
    }

    public struct Point
    {
        public int X, Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"Point {{X: {X}, Y: {Y}}}";
        }
    }
}