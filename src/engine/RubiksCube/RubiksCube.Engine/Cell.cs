using System.Numerics;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public class Cell
    {
        public Color Color { get; }
        
        public Vector3 Position { get; private set; }

        public Vector3 Normal { get; private set; }

        public Matrix4x4 Rotation { get; private set; }

        public Cell(Color color, Vector3 position)
        {
            Color = color;
            Position = position;
            Rotation = Matrix4x4.Identity;
            Normal = Vector3.UnitZ;
        }

        public void Rotate(Matrix4x4 rotation)
        {
            Position = Vector3.Transform(Position, rotation).Round();
            Normal = Vector3.TransformNormal(Normal, rotation).Round();
            Rotation *= rotation;
        }
    }
}
