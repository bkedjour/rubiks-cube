using System.Collections.Generic;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public class WhiteCross : Step
    {
        public WhiteCross(IEnumerable<CubeMove> moves) : base(moves)
        {
        }

        public override void Execute(ICube cube)
        {
            throw new System.NotImplementedException();
        }
    }
}
