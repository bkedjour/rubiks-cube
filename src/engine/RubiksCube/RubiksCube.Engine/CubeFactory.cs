using System.Collections.Generic;

namespace RubiksCube.Engine
{
    public class CubeFactory
    {
        public ICube CreateCube()
        {
            var faces = new List<Face>
            {
                CreateFace(Color.Yellow, Side.Up),
                CreateFace(Color.White, Side.Down),
                CreateFace(Color.Orange, Side.Right),
                CreateFace(Color.Red, Side.Left),
                CreateFace(Color.Green, Side.Front),
                CreateFace(Color.Blue, Side.Back),
            };

            return new Cube(faces);
        }

        private Face CreateFace(Color color, Side side)
        {
            var cells = CreateCells(color);
            return new Face(cells) {Side = side};
        }

        private static Cell[,] CreateCells(Color color)
        {
            return new[,]
            {
                {new Cell(color), new Cell(color), new Cell(color)},
                {new Cell(color), new Cell(color), new Cell(color)},
                {new Cell(color), new Cell(color), new Cell(color)}
            };
        }
    }
}
