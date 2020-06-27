using System;
using System.Linq;
using RubiksCube.Engine;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Manipulation
{
    public class WhiteCross : Step
    {
        public WhiteCross(ICube cube) : base(cube)
        {
        }

        public override void Execute()
        {
            var whiteCell = GetWhiteEdgeCell();

            while (whiteCell != null)
            {
                MatchCenterColor(whiteCell);

                RotateWhiteCell(whiteCell);

                whiteCell = GetWhiteEdgeCell();
            }

            base.Execute();
        }

        private Cell GetWhiteEdgeCell()
        {
            var a = Cube.GetFace(Side.Up).Cells
                .Where(c => c.Color == Color.White && (c.Position.X == 0 || c.Position.Z == 0)).ToList();

            return a.FirstOrDefault();
        }

        private void MatchCenterColor(Cell whiteCell)
        {
            var cellToMatch = Cube.Cells.Single(c => c.Position == whiteCell.Position && c.Color != Color.White);

            var center = Cube.Cells.Single(c => c.Position.Y == 0 && (c.Position.X == 0 || c.Position.Z == 0) && c.Color == cellToMatch.Color);

            var depthDiff = (cellToMatch.Position.Z == 0 && center.Position.Z == 0)
                ? cellToMatch.Position.X + center.Position.X
                : cellToMatch.Position.Z + center.Position.Z;


            if (Math.Abs(depthDiff) > 1)
                return;

            if (depthDiff == 0)
            {
                Cube.Move(Side.Up, Direction.Clockwise);
                Cube.PlayNextMove();
                Cube.Move(Side.Up, Direction.Clockwise);
                Cube.PlayNextMove();

                return;
            }

            Direction direction;
            if (cellToMatch.Normal.Z != 0)
            {
                direction = cellToMatch.Normal.Z + center.Normal.X == 0
                    ? Direction.Clockwise
                    : Direction.Counterclockwise;
            }
            else
            {
                direction = cellToMatch.Normal.X + center.Normal.Z == 0
                    ? Direction.Counterclockwise
                    : Direction.Clockwise;
            }

            Cube.Move(Side.Up, direction);
            Cube.PlayNextMove();
        }

        private void RotateWhiteCell(Cell whiteCell)
        {
            Side side;

            if (whiteCell.Position.Z == 0)
                side = whiteCell.Position.X > 0 ? Side.Right : Side.Left;
            else side = whiteCell.Position.Z > 0 ? Side.Front : Side.Back;

            Cube.Move(side, Direction.Clockwise);
            Cube.PlayNextMove();
            Cube.Move(side, Direction.Clockwise);
            Cube.PlayNextMove();
        }
    }
}
