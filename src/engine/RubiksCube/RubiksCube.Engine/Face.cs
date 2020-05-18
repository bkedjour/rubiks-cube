using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RubiksCube.Engine
{
    public class Face
    {
        public IReadOnlyList<Cell> Cells { get; }

        public Side Side { get; set; }

        public Status Status
        {
            get
            {
                var color = Cells.First().Color;
                return Cells.All(cell => cell.Color == color) ? Status.Solved : Status.NotSolved;
            }
        }

        public Face(IReadOnlyList<Cell> cells)
        {
            Cells = cells;
        }

        public static Matrix4x4 GetFaceRotation(Side side)
        {
            switch (side)
            {
                case Side.Front:
                    return Matrix4x4.Identity;
                case Side.Back:
                    return Matrix4x4.CreateRotationY(180.ToRadians());
                case Side.Right:
                    return Matrix4x4.CreateRotationY(90.ToRadians());
                case Side.Left:
                    return Matrix4x4.CreateRotationY(-90.ToRadians());
                case Side.Up:
                    return Matrix4x4.CreateRotationX(-90.ToRadians());
                case Side.Down:
                    return Matrix4x4.CreateRotationX(90.ToRadians());
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }
    }
}
