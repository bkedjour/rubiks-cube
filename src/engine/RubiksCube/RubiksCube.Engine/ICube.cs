namespace RubiksCube.Engine
{
    public interface ICube
    {
        void Shuffle();

        void Rotate(Axis axis, int angle);

        void Move(Side side, Direction direction);
    }
}
