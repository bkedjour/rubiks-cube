using System.Numerics;
using Veldrid;

namespace RubiksCube.Ui
{
    public struct VertexInfo
    {
        public Vector3 Position; // This is the position, in normalized device coordinates.
        public RgbaFloat Color;
        public Vector2 Uv;

        public const uint SizeInBytes = 36;

        public VertexInfo(Vector3 position, RgbaFloat color, Vector2 uv)
        {
            Position = position;
            Color = color;
            Uv = uv;
        }
    }
}
