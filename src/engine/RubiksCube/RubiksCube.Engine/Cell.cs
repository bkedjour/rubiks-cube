using System;
using System.Numerics;

namespace RubiksCube.Engine
{
    public class Cell
    {
        public Color Color { get; }
        
        public Vector3 Position { get; private set; }

        public Side Side { get; }

        public Matrix4x4 Rotation { get; private set; }

        public Cell(Color color, Vector3 position, Side side)
        {
            Color = color;
            Position = position;
            Side = side;
            Rotation = Matrix4x4.Identity;
        }

        public void Rotate(Matrix4x4 rotation)
        {
            var transform = Vector3.Transform(Position, rotation);
            Position = new Vector3((float) Math.Round(transform.X), (float) Math.Round(transform.Y),
                (float) Math.Round(transform.Z));

            Rotation *= rotation;
        }
    }
}
