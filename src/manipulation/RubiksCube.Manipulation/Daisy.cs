using System.Collections.Generic;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public class Daisy : Step
    {
        public Daisy(IEnumerable<CubeMove> moves) : base(moves)
        {
        }

        public override void Execute(ICube cube)
        {
            //find the 4 edge white pieces
            
        }
    }
}
