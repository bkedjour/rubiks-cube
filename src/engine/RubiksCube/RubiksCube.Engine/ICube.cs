using System.Collections.Generic;

namespace RubiksCube.Engine
{
    public interface ICube
    {
        IEnumerable<Face> Faces { get; }

        Status Status { get; }

        void Shuffle();

        void Rotate(Axis axis, int angle);

        void Move(Side side, Direction direction);
    }
}
