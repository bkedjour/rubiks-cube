using System.Numerics;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public class Cell
    {
        public Color Color { get; }

        public Vector3 Position { get; private set; }

        public Vector3 InitialPosition { get; }

        public Vector3 Normal { get; private set; }

        public RotationInfo RotationInfo { get; private set; }

        public bool HighLighted { get; set; }

        public int Id { get; }

        private static int _id;

        public Cell(Color color, Vector3 position)
        {
            Color = color;
            Position = position;
            InitialPosition = position;
            Normal = Vector3.UnitZ;

            RotationInfo = new RotationInfo();

            Id = _id++;
        }

        public void Rotate(RotationInfo rotationInfo)
        {
            Position = Vector3.Transform(Position, rotationInfo.RotationMatrix).Round();
            Normal = Vector3.TransformNormal(Normal, rotationInfo.RotationMatrix).Round();

            RotationInfo.RotationMatrix *= rotationInfo.RotationMatrix;
            RotationInfo.Axis = rotationInfo.Axis;
            RotationInfo.Angle = rotationInfo.Angle;
        }

        public Cell Clone()
        {
            var clone = (Cell) MemberwiseClone();
            clone.RotationInfo = RotationInfo.Clone();
            return clone;
        }

        public override string ToString()
        {
            return $"P:{Position} N:{Normal} C:{Color}";
        }
    }
}
