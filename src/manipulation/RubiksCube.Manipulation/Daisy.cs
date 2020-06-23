using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RubiksCube.Engine;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Manipulation
{
    public class Daisy : Step
    {
        private Dictionary<Tuple<Vector3, Vector3, Vector3>, Action> _playBook;

        public Daisy(ICube cube, IList<CubeMove> moves) : base(cube, moves)
        {
            _playBook = new Dictionary<Tuple<Vector3, Vector3, Vector3>, Action>();
            InitializeDaisyPlayBook();
        }

        public override void Execute()
        {
            PutYellowFaceOnTop();

            CreateDaisy();

            base.Execute();
        }

        private void CreateDaisy()
        {
            while (GetWhiteEdgeCells().Any())
            //for (int i = 0; i < 10; i++)
            {
                var targetCell = GetTopCrossNonWhiteCells().First();
                var whiteCell = GetWhiteEdgeCells().First();

                

                var key = new Tuple<Vector3, Vector3, Vector3>(targetCell.Position, whiteCell.Position, whiteCell.Normal);

                if (!_playBook.ContainsKey(key))
                {
                    targetCell.HighLighted = true;
                    whiteCell.HighLighted = true;
                    continue;
                }

                _playBook[key].Invoke();

                while (Cube.HasNextMove)
                    Cube.PlayNextMove();
            }

            foreach (var move in Cube.GetMoves())
            {
                Moves.Add(move);
            }
        }

        private void PutYellowFaceOnTop()
        {
            if (Cube.GetFace(Side.Up).Cells.Single(c => c.Position == Vector3.UnitY).Color == Color.Yellow)
                return;

            if (Cube.GetFace(Side.Front).Cells.Single(c => c.Position == Vector3.UnitZ).Color == Color.Yellow)
            {
                Moves.Add(new CubeMove(new RotationInfo(Axis.X, -90)));
                return;
            }

            if (Cube.GetFace(Side.Down).Cells.Single(c => c.Position == -Vector3.UnitY).Color == Color.Yellow)
            {
                Moves.Add(new CubeMove(new RotationInfo(Axis.X, -90)));
                Moves.Add(new CubeMove(new RotationInfo(Axis.X, -90)));
                return;
            }

            if (Cube.GetFace(Side.Back).Cells.Single(c => c.Position == -Vector3.UnitZ).Color == Color.Yellow)
            {
                Moves.Add(new CubeMove(new RotationInfo(Axis.X, 90)));
                return;
            }
            
            if (Cube.GetFace(Side.Right).Cells.Single(c => c.Position == Vector3.UnitX).Color == Color.Yellow)
            {
                Moves.Add(new CubeMove(new RotationInfo(Axis.Z, 90)));
                return;
            }
            
            if (Cube.GetFace(Side.Left).Cells.Single(c => c.Position == -Vector3.UnitX).Color == Color.Yellow)
            {
                Moves.Add(new CubeMove(new RotationInfo(Axis.Z, -90)));
                return;
            }
        }

        private List<Cell> GetTopCrossNonWhiteCells()
        {
            var topFace = Cube.GetFace(Side.Up);
            var cross = new List<Cell>
            {
                topFace.Cells.SingleOrDefault(c => c.Position.X == 0 && c.Position.Z == 1 && c.Color != Color.White),
                topFace.Cells.SingleOrDefault(c => c.Position.X == 1 && c.Position.Z == 0 && c.Color != Color.White),
                topFace.Cells.SingleOrDefault(c => c.Position.X == -1 && c.Position.Z == 0 && c.Color != Color.White),
                topFace.Cells.SingleOrDefault(c => c.Position.X == 0 && c.Position.Z == -1 && c.Color != Color.White)
            };

            cross.RemoveAll(c => c == null);

            return cross;
        }

        private List<Cell> GetWhiteEdgeCells()
        {
            var whiteEdgeCells = new List<Cell>();

            var front = Cube.Cells.Where(c =>
                c.Position.Z == 1 &&
                (c.Position.X == 0 && c.Position.Y != 0 && c.Normal.Y != 1 || c.Position.Y == 0 && c.Position.X != 0) &&
                c.Color == Color.White).ToList();

            whiteEdgeCells.AddRange(front);

            var middle = Cube.Cells.Where(c =>
                c.Position.Z == 0 && c.Position.Y != 0 && c.Position.X != 0 && c.Normal.Y != 1 && c.Color == Color.White).ToList();

            whiteEdgeCells.AddRange(middle);

            var back = Cube.Cells.Where(c =>
                c.Position.Z == -1 &&
                (c.Position.X == 0 && c.Position.Y != 0 && c.Normal.Y != 1 || c.Position.Y == 0 && c.Position.X != 0) &&
                c.Color == Color.White).ToList();

            whiteEdgeCells.AddRange(back);
            return whiteEdgeCells;
        }

        private void InitializeDaisyPlayBook()
        {
            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(0, 1, 1), new Vector3(0, -1, 1), -Vector3.UnitY),
                () =>
                {
                    Cube.Move(Side.Front, Direction.Clockwise);
                    Cube.Move(Side.Front, Direction.Clockwise);
                });

            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(1, 1, 0), new Vector3(-1, -1, 0), -Vector3.UnitY),
                () =>
                {
                    Cube.Move(Side.Down, Direction.Counterclockwise);
                    Cube.Move(Side.Down, Direction.Counterclockwise);
                    Cube.Move(Side.Right, Direction.Clockwise);
                    Cube.Move(Side.Right, Direction.Clockwise);
                });

            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(1, 1, 0), new Vector3(0, -1, 1), -Vector3.UnitY),
                () =>
                {
                    Cube.Move(Side.Down, Direction.Clockwise);
                    Cube.Move(Side.Right, Direction.Clockwise);
                    Cube.Move(Side.Right, Direction.Clockwise);
                });

            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(-1, 1, 0), new Vector3(0, -1, 1), -Vector3.UnitY),
                () =>
                {
                    Cube.Move(Side.Front, Direction.Counterclockwise);
                    Cube.Move(Side.Up, Direction.Counterclockwise);
                    Cube.Move(Side.Front, Direction.Counterclockwise);
                });
        }
    }
}
