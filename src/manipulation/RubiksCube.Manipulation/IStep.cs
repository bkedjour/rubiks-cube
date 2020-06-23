namespace RubiksCube.Manipulation
{
    public interface IStep
    {
        void Execute();

        IStep SetNext(IStep step);
    }
}
