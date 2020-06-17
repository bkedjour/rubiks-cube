using System.Collections.Generic;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public class Cfop : ISolver
    {
        public IEnumerable<CubeMove> Solve(ICube cube)
        {
            IEnumerable<CubeMove> moves = new List<CubeMove>();

            (new Daisy(moves)
                    .SetNext(new WhiteCross(moves))
                ).Execute(cube);

            return moves;
        }
    }
}
