using System.Collections.Generic;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public class WhiteCross : Step
    {
        public WhiteCross(ICube cube, IList<CubeMove> moves) : base(cube, moves)
        {
        }
    }
}
