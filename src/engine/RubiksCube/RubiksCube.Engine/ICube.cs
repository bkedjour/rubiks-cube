using System.Collections.Generic;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public interface ICube
    {
        Face GetFace(Side side);

        Status Status { get; }
        
        void Rotate(Axis axis, int angle);

        void Move(Side side, Direction direction);

        bool HasNextMove { get; }

        void PlayNextMove();

        void HighLight(IEnumerable<Cell> cells);
    }
}
