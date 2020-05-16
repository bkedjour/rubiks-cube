using System.Numerics;
using Veldrid;

namespace RubiksCube.Ui
{
    public struct VertexPositionColor
    {
        public Vector3 Position; // This is the position, in normalized device coordinates.
        public RgbaFloat Color; // This is the color of the vertex.

        public const uint SizeInBytes = 28;

        public VertexPositionColor(Vector3 position, RgbaFloat color)
        {
            Position = position;
            Color = color;
        }
    }
}
