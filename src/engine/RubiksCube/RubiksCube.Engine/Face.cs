using System.Linq;

namespace RubiksCube.Engine
{
    public class Face
    {
        public Cell[,] Cells { get; }

        public Side Side { get; set; }

        public Status Status
        {
            get
            {
                var color = Cells[0, 0].Color;
                return Cells.Cast<Cell>().All(cell => cell.Color == color) ? Status.Solved : Status.NotSolved;
            }
        }

        public Face(Cell[,] cells)
        {
            Cells = cells;
        }
    }
}
