using System.Collections.Generic;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public interface ISolver
    {
        IList<CubeMove> Solve(ICube cube);
    }
}
