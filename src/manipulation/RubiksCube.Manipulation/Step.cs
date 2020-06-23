using System.Collections.Generic;
using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public abstract class Step : IStep
    {
        protected readonly ICube Cube;
        protected IList<CubeMove> Moves;
        protected IStep NextStep;

        protected Step(ICube cube, IList<CubeMove> moves)
        {
            Cube = cube;
            Moves = moves;
        }

        public virtual void Execute()
        {
            NextStep?.Execute();
        }

        public IStep SetNext(IStep step)
        {
            NextStep = step;
            return step;
        }
    }
}
