using System.Numerics;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public class Cell
    {
        public Color Color { get; }

        public Vector3 Position { get; private set; }

        public Vector3 Normal { get; private set; }

        public RotationInfo RotationInfo { get; }

        public Cell(Color color, Vector3 position)
        {
            Color = color;
            Position = position;
            Normal = Vector3.UnitZ;

            RotationInfo = new RotationInfo();
        }

        public void Rotate(RotationInfo rotationInfo)
        {
            Position = Vector3.Transform(Position, rotationInfo.RotationMatrix).Round();
            Normal = Vector3.TransformNormal(Normal, rotationInfo.RotationMatrix).Round();

            RotationInfo.RotationMatrix *= rotationInfo.RotationMatrix;
            RotationInfo.Axis = rotationInfo.Axis;
            RotationInfo.Angle = rotationInfo.Angle;
        }
    }
}
