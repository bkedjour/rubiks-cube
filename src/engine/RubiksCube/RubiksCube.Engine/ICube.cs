using System.Collections.Generic;

namespace RubiksCube.Engine
{
    public interface ICube
    {
        IReadOnlyList<Cell> Cells { get; }

        Face GetFace(Side side);

        Status Status { get; }

        void Shuffle();

        void Rotate(Axis axis, int angle);

        void Move(Side side, Direction direction);
    }
}
