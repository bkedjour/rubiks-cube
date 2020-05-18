using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RubiksCube.Engine
{
    public class Cube : ICube
    {
        public IReadOnlyList<Cell> Cells { get; }

        public Cube(IReadOnlyList<Cell> cells)
        {
            Cells = cells;
        }
        
        public Face GetFace(Side side)
        {
            return new Face(Cells.Where(c => c.Side == side).ToList()) {Side = side};
        }

        public Status Status
        {
            get
            {
                var faces = new List<Face>
                {
                    GetFace(Side.Front), GetFace(Side.Right), GetFace(Side.Back), GetFace(Side.Left),
                    GetFace(Side.Up), GetFace(Side.Down)
                };
                return faces.All(f => f.Status == Status.Solved) ? Status.Solved : Status.NotSolved;
            }
        }


        public void Shuffle()
        {
            throw new System.NotImplementedException();
        }

        public void Rotate(Axis axis, int angle)
        {
            Matrix4x4 rotation;
            switch (axis)
            {
                case Axis.X:
                    rotation = Matrix4x4.CreateRotationX(angle.ToRadians());
                    break;
                case Axis.Y:
                    rotation = Matrix4x4.CreateRotationY(angle.ToRadians());
                    break;
                case Axis.Z:
                    rotation = Matrix4x4.CreateRotationZ(angle.ToRadians());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }

            foreach (var cell in Cells)
                cell.Rotate(rotation);
        }

        public void Move(Side side, Direction direction)
        {
            var cells = GetSideCells(side);
            var rotationValue = 90.ToRadians();

            foreach (var cell in cells)
            {
                //cell.Color = Color.Pink;
                switch (side)
                {
                    case Side.Front:
                        cell.Rotate(
                            Matrix4x4.CreateRotationZ(direction == Direction.Clockwise ? -rotationValue : rotationValue));
                        break;
                    case Side.Back:
                        cell.Rotate(
                            Matrix4x4.CreateRotationZ(direction == Direction.Clockwise ? rotationValue : -rotationValue));
                        break;
                    case Side.Right:
                        cell.Rotate(
                            Matrix4x4.CreateRotationX(direction == Direction.Clockwise ? -rotationValue : rotationValue));
                        break;
                    case Side.Left:
                        cell.Rotate(
                            Matrix4x4.CreateRotationX(direction == Direction.Clockwise ? rotationValue : -rotationValue));
                        break;
                    case Side.Up:
                        cell.Rotate(
                            Matrix4x4.CreateRotationY(direction == Direction.Clockwise ? -rotationValue : rotationValue));
                        break;
                    case Side.Down:
                        cell.Rotate(
                            Matrix4x4.CreateRotationY(direction == Direction.Clockwise ? rotationValue : -rotationValue));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(side), side, null);
                }
            }
        }

        private IReadOnlyList<Cell> GetSideCells(Side side)
        {
            switch (side)
            {
                case Side.Front:
                    return Cells.Where(c => c.Position.Z == 1).ToList();
                case Side.Back:
                    return Cells.Where(c => c.Position.Z == -1).ToList();
                case Side.Right:
                    return Cells.Where(c => c.Position.X == 1).ToList();
                case Side.Left:
                    return Cells.Where(c => c.Position.X == -1).ToList();
                case Side.Up:
                    return Cells.Where(c => c.Position.Y == 1).ToList();
                case Side.Down:
                    return Cells.Where(c => c.Position.Y == -1).ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }
    }
}
