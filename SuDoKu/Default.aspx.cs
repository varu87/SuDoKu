using System;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SuDoKu_Generator;

namespace SuDoKu
{
    public partial class _Default : System.Web.UI.Page
    {
        /// <summary>
        /// The puzzle grid
        /// </summary>
        string PuzzleGrid
        {
            get
            {
                return ViewState["puzzleGrid"].ToString();
            }
            set
            {
                ViewState["puzzleGrid"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                NewGame();
        }

        /// <summary>
        /// Create the new game
        /// </summary>
        public void NewGame()
        {
            GeneratePuzzle();
            DrawGrid(PuzzleGrid);
        }

        /// <summary>
        /// Generate the puzzle grid
        /// </summary>
        void GeneratePuzzle()
        {
            Logic l = new Logic();
            PuzzleGrid = l.GeneratePuzzle();
        }

        /// <summary>
        /// Draw the grid
        /// </summary>
        /// <param name="puzzleString"></param>
        void DrawGrid(string puzzleString)
        {
            int holesCount = puzzleString.Count(c => c.Equals('0'));

            HtmlTableRow tRow;
            HtmlTableCell tCell;
            TextBox cell;
            int index = 0;

            for (int row = 0; row < 9; row++)
            {
                tRow = new HtmlTableRow();
                for (int column = 0; column < 9; column++)
                {
                    tCell = new HtmlTableCell();
                    cell = new TextBox();
                    cell.ID = string.Format("cell{0}{1}", row, column);
                    cell.MaxLength = 1;

                    if (puzzleString.Substring(index, 1) != "0")
                    {
                        cell.Text = puzzleString.Substring(index, 1);
                        cell.Enabled = false;
                        cell.ForeColor = Color.Black;
                    }
                    else
                    {
                        cell.ForeColor = Color.BlueViolet;
                        cell.Attributes.Add("autocomplete", "off");
                        cell.Attributes.Add("onkeyup", string.Format("javascript:ValidateCell('{0}', '{1}', event)", cell.ID, holesCount));
                        cell.Attributes.Add("onfocus", string.Format("javascript:CellInFocus('{0}')", cell.ID));
                    }

                    tCell.Controls.Add(cell);
                    tRow.Cells.Add(tCell);
                    index++;
                }
                SuDoKuGrid.Rows.Add(tRow);
            }
        }

        protected void btnNewGame_OnClick(object o, EventArgs e)
        {
            NewGame();
        }
    }
}