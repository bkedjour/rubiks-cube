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
            return side switch
            {
                Side.Front => Matrix4x4.Identity,
                Side.Back => Matrix4x4.CreateRotationY(180.ToRadians()),
                Side.Right => Matrix4x4.CreateRotationY(90.ToRadians()),
                Side.Left => Matrix4x4.CreateRotationY(-90.ToRadians()),
                Side.Up => Matrix4x4.CreateRotationX(-90.ToRadians()),
                Side.Down => Matrix4x4.CreateRotationX(90.ToRadians()),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }
    }
}
