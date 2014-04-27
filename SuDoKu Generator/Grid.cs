using System;
using System.Collections.Generic;
using System.Text;

namespace SuDoKu_Generator
{
    public class Grid
    {
        int[] CUBE_LOOKUP = new int[6] { 1, -1, -2, 2, 1, -1 };
        public static Random r = new Random();

        public Cell[,] Puzzle { get; set; }
        public Level Difficulty { get; set; }
        bool HasChanged { get; set; }

        /// <summary>
        /// The first cell in the puzzle grid
        /// </summary>
        public Cell First
        {
            get
            {
                return this.Puzzle[0, 0];
            }
        }

        /// <summary>
        /// The last cell in the puzzle grid
        /// </summary>
        public Cell Last
        {
            get
            {
                return this.Puzzle[8, 8];
            }
        }

        /// <summary>
        /// Initialize the puzzle grid
        /// </summary>
        public Grid()
        {
            this.Puzzle = new Cell[9, 9];
            for (int row = 0; row < 9; row++)
                for (int column = 0; column < 9; column++)
                    this.Puzzle[row, column] = new Cell(row, column);
        }

        /// <summary>
        /// Generate the grid based on a difficulty level
        /// </summary>
        /// <param name="difficulty"></param>
        public Grid(Level difficulty)
        {
            this.Difficulty = difficulty;
            this.Puzzle = new Cell[9, 9];
            for (int row = 0; row < 9; row++)
                for (int column = 0; column < 9; column++)
                    this.Puzzle[row, column] = new Cell(row, column);
        }

        /// <summary>
        /// Get the next empty cell
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns>Cell</returns>
        public Cell GetCell(Cell currentCell)
        {
            int row = currentCell.Index.Row;
            int column = currentCell.Index.Column;

            if (row == 0 && column == 0)
                return this.Puzzle[row, column + 1];
            if (row == 8 && column == 8)
                return this.Last;

            Cell previousCell = PreviousCell(currentCell);

            if (string.IsNullOrEmpty(previousCell.Digit))
                return previousCell;
            else if (string.IsNullOrEmpty(currentCell.Digit))
                return currentCell;
            else
                return NextCell(currentCell);
        }

        /// <summary>
        /// Get the cell previous to the current cell
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns>Cell</returns>
        public Cell PreviousCell(Cell currentCell)
        {
            int row = currentCell.Index.Row;
            int column = currentCell.Index.Column;

            if (column > 0)
                return this.Puzzle[row, column - 1];
            else
                return this.Puzzle[row - 1, 8];
        }

        /// <summary>
        /// Get the next cell to the current cell
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns>Cell</returns>
        public Cell NextCell(Cell currentCell)
        {
            int row = currentCell.Index.Row;
            int column = currentCell.Index.Column;

            if (column == 8)
                return this.Puzzle[row + 1, 0];
            else
                return this.Puzzle[row, column + 1];
        }

        /// <summary>
        /// Generate the terminal pattern
        /// </summary>
        /// <returns>Terminal Pattern</returns>
        public Cell[,] GenerateTerminalPattern()
        {
            ValidatePattern(this.First);
            return this.Puzzle;
        }

        /// <summary>
        /// Validate if the current cell fits in the pattern
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns>True if the pattern is valid</returns>
        bool ValidatePattern(Cell currentCell)
        {
            if (!string.IsNullOrEmpty(this.Last.Digit))
                return true;

            if (ValidateCell(currentCell))
            {
                while (currentCell.ValidDigits.Length > 0)
                {
                    currentCell.Digit = currentCell.ValidDigits.Substring(r.Next(currentCell.ValidDigits.Length), 1);
                    currentCell.ValidDigits = currentCell.ValidDigits.Replace(currentCell.Digit, string.Empty);

                    if (ValidatePattern(GetCell(currentCell)))
                        return true;
                }
            }
            currentCell.RestrictedDigits = currentCell.Digit;
            currentCell.Digit = string.Empty;
            currentCell.ValidDigits = Utility.START_SEQUENCE;

            if (string.IsNullOrEmpty(Last.Digit))
                this.PreviousCell(currentCell).Digit = string.Empty;

            return false;
        }

        /// <summary>
        /// Validate if the current cell value fits the pattern
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns>True if the cell fits</returns>
        bool ValidateCell(Cell currentCell)
        {
            if (RestrictedDigits(currentCell).Length == 9)
                return false;

            ValidDigits(currentCell);
            return true;
        }

        /// <summary>
        /// Remove cells from the terminal pattern
        /// </summary>
        /// <param name="Sequence"></param>
        /// <returns>Puzzle</returns>
        public Cell[,] DigHoles(List<Point> Sequence)
        {
            int requiredGivensCount;
            int actualGivensCount;
            Cell currentCell;

            foreach (Point pt in Sequence)
                GenerateEasyGrid(this.Puzzle[pt.Row, pt.Column]);

            Sequence = Sequence.FindAll(pt => this.Puzzle[pt.Row, pt.Column].IsGiven == true);
            actualGivensCount = Sequence.Count;

            ReEvaluateGrid();

            switch (this.Difficulty)
            {
                case Level.Easy:
                    requiredGivensCount = r.Next(36, 49);
                    break;

                case Level.Medium:
                    requiredGivensCount = r.Next(32, 35);
                    break;

                case Level.Hard:
                    requiredGivensCount = r.Next(28, 31);
                    break;

                case Level.Evil:
                    requiredGivensCount = r.Next(22, 27);
                    break;

                default:
                    requiredGivensCount = r.Next(22, 49);
                    break;
            }

            foreach (Point pt in Sequence)
            {
                currentCell = this.Puzzle[pt.Row, pt.Column];
                Dig(currentCell);

                if (!currentCell.IsGiven)
                    actualGivensCount--;

                if (actualGivensCount <= requiredGivensCount)
                    break;
            }

            return this.Puzzle;
        }

        /// <summary>
        /// Create easy puzzle
        /// </summary>
        /// <param name="currentCell"></param>
        void GenerateEasyGrid(Cell currentCell)
        {
            string digit;
            currentCell.IsGiven = true;
            digit = currentCell.Digit;
            currentCell.RestrictedDigits = RestrictedDigits(currentCell);

            if (currentCell.RestrictedDigits.Length == 9)
            {
                currentCell.Digit = string.Empty;
                currentCell.IsGiven = false;
            }
        }

        /// <summary>
        /// Remove the current cell
        /// </summary>
        /// <param name="currentCell"></param>
        public void Dig(Cell currentCell)
        {
            string digit = currentCell.Digit;
            currentCell.Digit = string.Empty;
            currentCell.IsGiven = false;

            if (!IterateGrid())
            {
                currentCell.IsGiven = true;
                currentCell.Digit = digit;
            }

            CleanGrid();
        }

        /// <summary>
        /// Generate the restricted and valid digits of all the cells
        /// </summary>
        void ReEvaluateGrid()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    RestrictedDigits(this.Puzzle[row, column]);
                    ValidDigits(this.Puzzle[row, column]);
                }
            }
        }

        /// <summary>
        /// Iterate the grid and remove cells from the puzzle
        /// </summary>
        /// <returns>True if the grid satisfies the general rules</returns>
        bool IterateGrid()
        {
            this.HasChanged = true;
            bool invalid = false;
            Cell currentCell;

            while (this.HasChanged)
            {
                this.HasChanged = false;
                invalid = false;

                for (int row = 0; row < 9; row++)
                {
                    for (int column = 0; column < 9; column++)
                    {
                        currentCell = this.Puzzle[row, column];

                        if (!(currentCell.Digit.Length == 1))
                        {
                            RestrictedDigits(currentCell);

                            if (currentCell.RestrictedDigits.Length == 9)
                                return false;

                            ValidDigits(currentCell);

                            if (currentCell.ValidDigits.Length == 1)
                            {
                                currentCell.Digit = currentCell.ValidDigits;
                                this.HasChanged = true;
                            }
                            else
                                invalid = invalid || true;
                        }
                    }
                }
            }
            return !invalid;
        }

        /// <summary>
        /// Get the digits not allowed in the current cell
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns>Invalid digits</returns>
        string RestrictedDigits(Cell currentCell)
        {
            int row = currentCell.Index.Row;
            int column = currentCell.Index.Column;
            StringBuilder combination = new StringBuilder();

            for (int i = 0; i < 9; i++)
            {
                if (combination.ToString().IndexOf(this.Puzzle[row, i].Digit) == -1)
                    combination.Append(this.Puzzle[row, i].Digit);

                if (combination.ToString().IndexOf(this.Puzzle[i, column].Digit) == -1)
                    combination.Append(this.Puzzle[i, column].Digit);

                if (i == column)
                {
                    if (combination.ToString().IndexOf(this.Puzzle[row + CUBE_LOOKUP[row % 3], i + CUBE_LOOKUP[i % 3]].Digit) == -1)
                        combination.Append(this.Puzzle[row + CUBE_LOOKUP[row % 3], i + CUBE_LOOKUP[i % 3]].Digit);

                    if (combination.ToString().IndexOf(this.Puzzle[row + CUBE_LOOKUP[(row % 3) + 3], i + CUBE_LOOKUP[i % 3]].Digit) == -1)
                        combination.Append(this.Puzzle[row + CUBE_LOOKUP[(row % 3) + 3], i + CUBE_LOOKUP[i % 3]].Digit);
                }

                if (i == row)
                {
                    if (combination.ToString().IndexOf(this.Puzzle[i + CUBE_LOOKUP[i % 3], column + CUBE_LOOKUP[(column % 3) + 3]].Digit) == -1)
                        combination.Append(this.Puzzle[i + CUBE_LOOKUP[i % 3], column + CUBE_LOOKUP[(column % 3) + 3]].Digit);

                    if (combination.ToString().IndexOf(this.Puzzle[i + CUBE_LOOKUP[(i % 3) + 3], column + CUBE_LOOKUP[(column % 3) + 3]].Digit) == -1)
                        combination.Append(this.Puzzle[i + CUBE_LOOKUP[(i % 3) + 3], column + CUBE_LOOKUP[(column % 3) + 3]].Digit);
                }
            }
            return currentCell.RestrictedDigits = combination.ToString();
        }

        /// <summary>
        /// Get the digits valid for the current cell
        /// </summary>
        /// <param name="currentCell"></param>
        /// <returns>Valid digits</returns>
        string ValidDigits(Cell currentCell)
        {
            string startSequence = Utility.START_SEQUENCE;
            if (currentCell.RestrictedDigits.Length < 9)
            {
                for (int i = 0; i < currentCell.RestrictedDigits.Length; i++)
                    currentCell.ValidDigits = startSequence = startSequence.Replace(currentCell.RestrictedDigits.Substring(i, 1), string.Empty);

                return currentCell.ValidDigits;
            }
            else
                return currentCell.ValidDigits = string.Empty;
        }

        /// <summary>
        /// Reset the empty cells
        /// </summary>
        void CleanGrid()
        {
            for(int row=0;row<9;row++)
                for(int column=0;column<9;column++)
                    if(!this.Puzzle[row,column].IsGiven)
                        this.Puzzle[row,column].Digit=string.Empty;
        }
    }
}
