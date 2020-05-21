using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public interface ICube
    {
        Face GetFace(Side side);

        Status Status { get; }
        
        void Rotate(Axis axis, int angle);

        void Move(Side side, Direction direction);
    }
}
