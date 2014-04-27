using System.Collections.Generic;
using System.Text;

namespace SuDoKu_Generator
{
    public class Logic
    {
        /// <summary>
        /// Generate the puzzle grid with an arbitrary number of 'givens'
        /// </summary>
        /// <returns>Grid</returns>
        public string GeneratePuzzle()
        {
            Grid grid = new Grid();
            grid.GenerateTerminalPattern();
            return PuzzleGrid(grid.DigHoles(RandomSequence()));
        }

        /// <summary>
        /// Generate the puzzle grid based on a difficulty level
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns>Grid</returns>
        public string[] GeneratePuzzle(Level difficulty)
        {
            string[] puzzle = new string[2];
            Grid grid = new Grid(difficulty);
            puzzle[0] = TerminalPattern(grid.GenerateTerminalPattern());
            puzzle[1] = PuzzleGrid(grid.DigHoles(RandomSequence()));
            return puzzle;
        }

        /// <summary>
        /// Generate the terminal pattern
        /// </summary>
        /// <param name="terminalPattern"></param>
        /// <returns>Solution</returns>
        string TerminalPattern(Cell[,] terminalPattern)
        {
            StringBuilder solution = new StringBuilder();

            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    solution.Append(terminalPattern[row, column].Digit);
                }
            }

            return solution.ToString();
        }

        /// <summary>
        /// Get the sequence in which to create holes in the terminal pattern
        /// </summary>
        /// <returns>List</returns>
        List<Point> RandomSequence()
        {
            List<Point> sequence = new List<Point>();
            List<int> temp = new List<int>();
            int index, row, column;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    temp.Add((10 * i) + j);
                }
            }
            while (temp.Count > 0)
            {
                index = temp[Grid.r.Next(temp.Count)];
                row = index / 10;
                column = index % 10;
                sequence.Add(new Point(row, column));
                temp.Remove(index);
            }
            return sequence;
        }

        /// <summary>
        /// Convert the puzzle grid to string
        /// </summary>
        /// <param name="puzzleGrid"></param>
        /// <returns>Grid</returns>
        string PuzzleGrid(Cell[,] puzzleGrid)
        {
            StringBuilder puzzle = new StringBuilder();

            for (int row = 0; row < 9; row++)
                for (int column = 0; column < 9; column++)
                    if (string.IsNullOrEmpty(puzzleGrid[row, column].Digit))
                        puzzle.Append("0");
                    else
                        puzzle.Append(puzzleGrid[row, column].Digit);

            return puzzle.ToString();
        }
    }    
}
