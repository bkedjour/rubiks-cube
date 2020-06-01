using System;
using System.Collections.Generic;
using System.Linq;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public class Face
    {
        public IReadOnlyList<Cell> Cells { get; }

        public Side Side { get; }

        public Status Status
        {
            get
            {
                var color = Cells.First().Color;
                return Cells.All(cell => cell.Color == color) ? Status.Solved : Status.NotSolved;
            }
        }

        public Face(IReadOnlyList<Cell> cells, Side side)
        {
            Cells = cells;
            Side = side;
        }

        public static RotationInfo GetFaceRotation(Side side)
        {
            return side switch
            {
                Side.Front => new RotationInfo(),
                Side.Back => new RotationInfo(Axis.Y, 180),
                Side.Right => new RotationInfo(Axis.Y, 90),
                Side.Left => new RotationInfo(Axis.Y, -90),
                Side.Up => new RotationInfo(Axis.X, -90),
                Side.Down => new RotationInfo(Axis.X, 90),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }
    }
}
