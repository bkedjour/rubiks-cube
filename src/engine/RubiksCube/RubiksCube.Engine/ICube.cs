using System.Collections.Generic;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public interface ICube
    {
        IReadOnlyList<Cell> Cells { get; }

        Face GetFace(Side side);

        IReadOnlyList<Cell> GetSidePieces(Side side);

        Status Status { get; }
        
        void Rotate(Axis axis, int angle);

        void Move(Side side, Direction direction);
        
        void Move(CubeMove move);

        bool HasNextMove { get; }

        void PlayNextMove();

        void HighLight(IEnumerable<Cell> cells);

        IReadOnlyList<CubeMove> GetMoves();

        ICube Clone();
    }
}
