using System.Collections.Generic;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public abstract class Step : IStep
    {
        protected IEnumerable<CubeMove> Moves;
        protected IStep NextStep;

        protected Step(IEnumerable<CubeMove> moves)
        {
            Moves = moves;
        }

        public abstract void Execute(ICube cube);

        public IStep SetNext(IStep step)
        {
            NextStep = step;
            return step;
        }
    }
}
