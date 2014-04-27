
namespace SuDoKu_Generator
{
    public class Cell
    {
        public Point Index { get; set; }
        public string Digit { get; set; }
        public string ValidDigits { get; set; }
        public string RestrictedDigits { get; set; }
        public bool IsGiven { get; set; }

        public Cell()
        {
            this.Index = new Point();
        }

        public Cell(int row, int column)
        {
            this.Index = new Point(row, column);
            this.Digit = string.Empty;
            this.ValidDigits = Utility.START_SEQUENCE;
            this.RestrictedDigits = string.Empty;
            this.IsGiven = true;
        }
    }

    public class Point
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Point()
        {
            this.Row = -1;
            this.Column = -1;
        }

        public Point(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }
    }
}
