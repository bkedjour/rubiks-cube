using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public class Cube : ICube
    {
        public Face GetFace(Side side) => new Face(_cells.Where(c => c.Side == side).ToList(), side);
        
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

        private readonly IReadOnlyList<Cell> _cells;

        public Cube(IReadOnlyList<Cell> cells)
        {
            _cells = cells;
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

            foreach (var cell in _cells)
                cell.Rotate(rotation);
        }

        public void Move(Side side, Direction direction)
        {
            var angle = 90.ToRadians();

            var rotation = side switch
            {
                Side.Front => Matrix4x4.CreateRotationZ(direction == Direction.Clockwise ? -angle : angle),
                Side.Back => Matrix4x4.CreateRotationZ(direction == Direction.Clockwise ? angle : -angle),
                Side.Right => Matrix4x4.CreateRotationX(direction == Direction.Clockwise ? -angle : angle),
                Side.Left => Matrix4x4.CreateRotationX(direction == Direction.Clockwise ? angle : -angle),
                Side.Up => Matrix4x4.CreateRotationY(direction == Direction.Clockwise ? -angle : angle),
                Side.Down => Matrix4x4.CreateRotationY(direction == Direction.Clockwise ? angle : -angle),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };

            foreach (var cell in GetSidePieces(side))
                cell.Rotate(rotation);
        }

        private IReadOnlyList<Cell> GetSidePieces(Side side)
        {
            return side switch
            {
                Side.Front => _cells.Where(c => Math.Abs(c.Position.Z - 1) < float.Epsilon).ToList(),
                Side.Back => _cells.Where(c => Math.Abs(c.Position.Z - (-1)) < float.Epsilon).ToList(),
                Side.Right => _cells.Where(c => Math.Abs(c.Position.X - 1) < float.Epsilon).ToList(),
                Side.Left => _cells.Where(c => Math.Abs(c.Position.X - (-1)) < float.Epsilon).ToList(),
                Side.Up => _cells.Where(c => Math.Abs(c.Position.Y - 1) < float.Epsilon).ToList(),
                Side.Down => _cells.Where(c => Math.Abs(c.Position.Y - (-1)) < float.Epsilon).ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }
    }
}
