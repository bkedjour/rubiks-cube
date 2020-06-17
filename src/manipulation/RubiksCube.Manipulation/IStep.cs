using System.Collections.Generic;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public interface IStep
    {
        void Execute(ICube cube);

        IStep SetNext(IStep step);
    }
}
