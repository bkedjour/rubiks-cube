using System.Collections.Generic;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public interface ISolver
    {
        IEnumerable<CubeMove> Solve(ICube cube);
    }
}
