using System;
using System.Collections.Generic;
using System.Linq;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public class Cube : ICube
    {
        private readonly LinkedList<CubeMove> _moves = new LinkedList<CubeMove>();
        private LinkedListNode<CubeMove> _head;

        public Face GetFace(Side side) => new Face(GetSideCells(side), side);
        
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

        public IReadOnlyList<Cell> Cells { get; }

        public Cube(IReadOnlyList<Cell> cells)
        {
            Cells = cells;
        }

        public void Rotate(Axis axis, int angle)
        {
            _moves.AddLast(new CubeMove(new RotationInfo(axis, angle)));
        }

        public void Move(Side side, Direction direction)
        {
            const int angle = 90;

            var negativeAngle = direction == Direction.Clockwise ? -angle : angle;
            var positiveAngle = direction == Direction.Clockwise ? angle : -angle;

            var (axis, rotationAngle) = side switch
            {
                Side.Front => (Axis.Z, negativeAngle),
                Side.Back => (Axis.Z, positiveAngle),
                Side.Right => (Axis.X, negativeAngle),
                Side.Left => (Axis.X, positiveAngle),
                Side.Up => (Axis.Y, negativeAngle),
                Side.Down => (Axis.Y, positiveAngle),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };

            var cells = GetSidePieces(side);
            _moves.AddLast(new CubeMove(side, new RotationInfo(axis, rotationAngle)));
        }

        public void Move(CubeMove move)
        {
            if (move == null) return;
            _moves.AddLast(move);
        }

        public bool HasNextMove => _head == null ? _moves.First != null : _head.Next != null;

        public void PlayNextMove()
        {
            if (!HasNextMove) return;

            _head = _head == null ? _moves.First : _head.Next;

            var move = _head.Value;
            var cells = move.Side == (Side) (-1) ? Cells : GetSidePieces(move.Side);
            foreach (var cell in cells)
                cell.Rotate(move.RotationInfo);
        }

        public void HighLight(IEnumerable<Cell> cells)
        {
            if (cells == null)
            {
                foreach (var cell in Cells)
                    cell.HighLighted = false;
                return;
            }

            foreach (var cell in cells)
                cell.HighLighted = true;
        }

        public IReadOnlyList<CubeMove> GetMoves()
        {
            return _moves.ToList();
        }

        public IReadOnlyList<Cell> GetSidePieces(Side side)
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

        private IReadOnlyList<Cell> GetSideCells(Side side)
        {
            return side switch
            {
                Side.Front => Cells.Where(c => Math.Abs(c.Normal.Z - 1) < float.Epsilon).ToList(),
                Side.Back => Cells.Where(c => Math.Abs(c.Normal.Z - (-1)) < float.Epsilon).ToList(),
                Side.Right => Cells.Where(c => Math.Abs(c.Normal.X - 1) < float.Epsilon).ToList(),
                Side.Left => Cells.Where(c => Math.Abs(c.Normal.X - (-1)) < float.Epsilon).ToList(),
                Side.Up => Cells.Where(c => Math.Abs(c.Normal.Y - 1) < float.Epsilon).ToList(),
                Side.Down => Cells.Where(c => Math.Abs(c.Normal.Y - (-1)) < float.Epsilon).ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }
    }
}
