using System.Collections.Generic;
using System.Linq;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public class Cfop : ISolver
    {
        public IList<CubeMove> Solve(ICube cube)
        {
            IList<CubeMove> moves = new List<CubeMove>();
            var workingCube = CloneCube(cube);

            var initialStep = new Daisy(workingCube, moves);
            initialStep.SetNext(new WhiteCross(workingCube, moves));

            initialStep.Execute();

            foreach (var cubeCell in cube.Cells)
            {
                var cloneCell = workingCube.Cells.Single(c => c.Id == cubeCell.Id);

                cubeCell.HighLighted = cloneCell.HighLighted;
            }

            return moves;
        }

        private ICube CloneCube(ICube cube)
        {
            return new Cube(cube.Cells.Select(cell => cell.Clone()).ToList());
        }
    }
}
