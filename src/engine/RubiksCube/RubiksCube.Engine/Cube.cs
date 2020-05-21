using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RubiksCube.Engine
{
    public class Cube : ICube
    {
        public IReadOnlyList<Cell> Cells { get; }

        public Face GetFace(Side side) => new Face(Cells.Where(c => c.Side == side).ToList()) { Side = side };

        public Status Status
        {
            get
            {
                var faces = new List<Face>();
                for (var i = 0; i < 6; i++)
                    faces.Add(GetFace((Side) i));
                
                return faces.All(f => f.Status == Status.Solved) ? Status.Solved : Status.NotSolved;
            }
        }

        public Cube(IReadOnlyList<Cell> cells)
        {
            Cells = cells;
        }

        public void Shuffle()
        {
            throw new System.NotImplementedException();
        }

        public void Rotate(Axis axis, int angle)
        {
            var rotation = axis switch
            {
                Axis.X => Matrix4x4.CreateRotationX(angle.ToRadians()),
                Axis.Y => Matrix4x4.CreateRotationY(angle.ToRadians()),
                Axis.Z => Matrix4x4.CreateRotationZ(angle.ToRadians()),
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };

            foreach (var cell in Cells)
                cell.Rotate(rotation);
        }

        public void Move(Side side, Direction direction)
        {
            var rotationAngle = 90.ToRadians();

            var rotation = side switch
            {
                Side.Front => Matrix4x4.CreateRotationZ(direction == Direction.Clockwise ? -rotationAngle : rotationAngle),
                Side.Back => Matrix4x4.CreateRotationZ(direction == Direction.Clockwise ? rotationAngle : -rotationAngle),
                Side.Right => Matrix4x4.CreateRotationX(direction == Direction.Clockwise ? -rotationAngle : rotationAngle),
                Side.Left => Matrix4x4.CreateRotationX(direction == Direction.Clockwise ? rotationAngle : -rotationAngle),
                Side.Up => Matrix4x4.CreateRotationY(direction == Direction.Clockwise ? -rotationAngle : rotationAngle),
                Side.Down => Matrix4x4.CreateRotationY(direction == Direction.Clockwise ? rotationAngle : -rotationAngle),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };

            foreach (var cell in GetSideCells(side))
                cell.Rotate(rotation);
        }

        private IReadOnlyList<Cell> GetSideCells(Side side)
        {
            return side switch
            {
                Side.Front => Cells.Where(c => Math.Abs(c.Position.Z - 1) < float.Epsilon).ToList(),
                Side.Back => Cells.Where(c => Math.Abs(c.Position.Z - (-1)) < float.Epsilon).ToList(),
                Side.Right => Cells.Where(c => Math.Abs(c.Position.X - 1) < float.Epsilon).ToList(),
                Side.Left => Cells.Where(c => Math.Abs(c.Position.X - (-1)) < float.Epsilon).ToList(),
                Side.Up => Cells.Where(c => Math.Abs(c.Position.Y - 1) < float.Epsilon).ToList(),
                Side.Down => Cells.Where(c => Math.Abs(c.Position.Y - (-1)) < float.Epsilon).ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }
    }
}
