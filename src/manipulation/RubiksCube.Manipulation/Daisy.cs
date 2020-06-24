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
        private readonly Dictionary<Tuple<Vector3, Vector3, Vector3>, Action> _playBook;

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
            {
                while (Cube.HasNextMove)
                    Cube.PlayNextMove();

                var topCrossNonWhiteCells = GetTopCrossNonWhiteCells();

                if (topCrossNonWhiteCells.Count == 0) break;

                Cell whiteCell = null;
                Cell targetCell = null;

                foreach (var cell in topCrossNonWhiteCells)
                {
                    targetCell = cell;
                    whiteCell = targetCell.Position.X != 0
                        ? GetWhiteEdgeCells().FirstOrDefault(c => c.Position.X == targetCell.Position.X && c.Normal.X == 0)
                        : GetWhiteEdgeCells().FirstOrDefault(c => c.Position.Z == targetCell.Position.Z && c.Normal.Z == 0);

                    if (whiteCell != null) break;
                }


                if (whiteCell is null)
                {
                    var verticalWhiteCells = new List<Cell>();
                    var horizontalWhiteCells = new List<Cell>();

                    foreach (var cell in topCrossNonWhiteCells)
                    {
                        targetCell = cell;
                        verticalWhiteCells = targetCell.Position.X != 0
                            ? GetWhiteEdgeCells()
                                .Where(c => c.Position.Z == 0 && c.Position.X == targetCell.Position.X &&
                                            c.Normal.X != 0).ToList()
                            : GetWhiteEdgeCells()
                                .Where(c => c.Position.X == 0 && c.Position.Z == targetCell.Position.Z &&
                                            c.Normal.Z != 0).ToList();

                        horizontalWhiteCells = targetCell.Position.X != 0
                            ? GetWhiteEdgeCells()
                                .Where(c => c.Position.Z != 0 && c.Position.X == targetCell.Position.X &&
                                            c.Normal.X != 0).ToList()
                            : GetWhiteEdgeCells()
                                .Where(c => c.Position.X != 0 && c.Position.Z == targetCell.Position.Z &&
                                            c.Normal.Z != 0).ToList();

                        whiteCell = verticalWhiteCells.FirstOrDefault();
                        if (whiteCell != null) break;
                    }
                
                    Side side;

                    if (verticalWhiteCells.Count > 1 && verticalWhiteCells.Any(c => c.Position.Y == -1))
                    {
                        side = Side.Down;
                    }
                    else if (verticalWhiteCells.Count > 0 && horizontalWhiteCells.Count > 0)
                    {
                        side = Side.Up;
                    }else
                    {
                        side = whiteCell?.Normal.Z switch
                        {
                            null => Side.Up,
                            1 => Side.Front,
                            -1 => Side.Back,
                            _ => whiteCell.Normal.X == 1 ? Side.Right : Side.Left
                        };
                    }


                    Cube.Move(side, Direction.Clockwise);

                    continue;
                }

                var key = new Tuple<Vector3, Vector3, Vector3>(targetCell.Position, whiteCell.Position, whiteCell.Normal);

                if (!_playBook.ContainsKey(key))
                {
                    Cube.Move(Side.Up, Direction.Clockwise);
                    continue;
                }

                _playBook[key].Invoke();
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
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(0, 1, 1), new Vector3(1, 0, 1), Vector3.UnitX),
                () =>
                {
                    Cube.Move(Side.Front, Direction.Counterclockwise);
                });

            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(0, 1, 1), new Vector3(-1, 0, 1), -Vector3.UnitX),
                () =>
                {
                    Cube.Move(Side.Front, Direction.Clockwise);
                });

            
            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(1, 1, 0), new Vector3(1, -1, 0), -Vector3.UnitY),
                () =>
                {
                    Cube.Move(Side.Right, Direction.Clockwise);
                    Cube.Move(Side.Right, Direction.Clockwise);
                });
            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(1, 1, 0), new Vector3(1, 0, 1), Vector3.UnitZ),
                () =>
                {
                    Cube.Move(Side.Right, Direction.Clockwise);
                });
            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(1, 1, 0), new Vector3(1, 0, -1), -Vector3.UnitZ),
                () =>
                {
                    Cube.Move(Side.Right, Direction.Counterclockwise);
                });

            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(0, 1, -1), new Vector3(0, -1, -1), -Vector3.UnitY),
                () =>
                {
                    Cube.Move(Side.Back, Direction.Clockwise);
                    Cube.Move(Side.Back, Direction.Clockwise);
                });
            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(0, 1, -1), new Vector3(1, 0, -1), Vector3.UnitX),
                () =>
                {
                    Cube.Move(Side.Back, Direction.Clockwise);
                });
            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(0, 1, -1), new Vector3(-1, 0, -1), -Vector3.UnitX),
                () =>
                {
                    Cube.Move(Side.Back, Direction.Counterclockwise);
                });

            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(-1, 1, 0), new Vector3(-1, -1, 0), -Vector3.UnitY),
                () =>
                {
                    Cube.Move(Side.Left, Direction.Clockwise);
                    Cube.Move(Side.Left, Direction.Clockwise);
                });
            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(-1, 1, 0), new Vector3(-1, 0, 1), Vector3.UnitZ),
                () =>
                {
                    Cube.Move(Side.Left, Direction.Counterclockwise);
                });
            _playBook.Add(
                new Tuple<Vector3, Vector3, Vector3>(new Vector3(-1, 1, 0), new Vector3(-1, 0, -1), -Vector3.UnitZ),
                () =>
                {
                    Cube.Move(Side.Left, Direction.Clockwise);
                });
        }
    }
}
