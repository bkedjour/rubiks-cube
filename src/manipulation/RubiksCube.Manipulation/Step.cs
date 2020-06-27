using RubiksCube.Engine;

namespace RubiksCube.Manipulation
{
    public abstract class Step : IStep
    {
        protected readonly ICube Cube;
        protected IStep NextStep;

        protected Step(ICube cube)
        {
            Cube = cube;
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
