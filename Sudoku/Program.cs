using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Sudoku
{
    class Program
    {
        public const string SudokuFilePath = "sudoku.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Reading file.");
            string absolutePath = Path.Combine(GetApplicationDirectory(), SudokuFilePath);
            if (File.Exists(absolutePath))
            {
                string fileContents = File.ReadAllText(absolutePath);
                Console.WriteLine("Generating board.");

                Board sudokuBoard = GetBoardFromString(fileContents);

                Console.WriteLine("Solving the board.");

                Stopwatch stopwatch = Stopwatch.StartNew();
                Board solvedBoard = sudokuBoard.Solve();

                stopwatch.Stop();

                                if (solvedBoard != null)
                                {
                                    Console.WriteLine($"Solving succesfull, it took {stopwatch.ElapsedMilliseconds} ms.");
                                    Console.Write(solvedBoard.ToString());
                                }
                                else
                                {
                                    Console.WriteLine($"Solving failed, it took {stopwatch.ElapsedMilliseconds} ms.");
                                }
            }
            else
            {
                Console.WriteLine($"Failed to read file at path \"{absolutePath}\".");
            }

            Console.ReadKey();
        }

        public static string GetApplicationDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static Board GetBoardFromString(string input)
        {
            string[] lines = input.Split(Environment.NewLine);

            Board sudokuBoard = new Board();

            for (int y = 0; y < lines.Length; y++)
            {
                string line = lines[y];
                string[] cells = line.Split(' ');
                for (int x = 0; x < cells.Length; x++)
                {
                    string cell = cells[x];
                    if (int.TryParse(cell, out int result))
                        sudokuBoard.Set(new Point(x, y), result);
                }
            }

            return sudokuBoard;
        }
    }
}