using System.Collections.Generic;
using System.Linq;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public class Cfop : ISolver
    {
        public IList<CubeMove> Solve(ICube cube)
        {
            var workingCube = cube.Clone();

            var initialStep = new Daisy(workingCube);
            initialStep.SetNext(new WhiteCross(workingCube));

            initialStep.Execute();

            foreach (var cubeCell in cube.Cells)
            {
                var cloneCell = workingCube.Cells.Single(c => c.Id == cubeCell.Id);

                cubeCell.HighLighted = cloneCell.HighLighted;
            }


            return workingCube.GetMoves().ToList();
        }
    }
}
