using System.Collections.Generic;
using System.Numerics;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public class CubeFactory
    {
        public ICube CreateCube()
        {
            var cells = new List<Cell>();

            cells.AddRange(CreateCells(Color.Yellow, Side.Up));
            cells.AddRange(CreateCells(Color.White, Side.Down));
            cells.AddRange(CreateCells(Color.Orange, Side.Right));
            cells.AddRange(CreateCells(Color.Red, Side.Left));
            cells.AddRange(CreateCells(Color.Green, Side.Front));
            cells.AddRange(CreateCells(Color.Blue, Side.Back));

            return new Cube(cells);
        }

        private static IEnumerable<Cell> CreateCells(Color color, Side side)
        {
            var cells = new List<Cell>();
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    var cell = new Cell(color, new Vector3(row - 1, (col - 1) * -1, 1));
                    cell.Rotate(Face.GetFaceRotation(side));

                    cells.Add(cell);
                }
            }

            return cells;
        }
    }
}
