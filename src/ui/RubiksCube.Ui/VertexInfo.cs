using System.Numerics;
using Veldrid;

namespace RubiksCube.Ui
{
    public struct VertexInfo
    {
        public Vector3 Position;
        public RgbaFloat Color;
        public Vector2 Uv;
        public float HighLighted;

        public const uint SizeInBytes = 40;

        public VertexInfo(Vector3 position, RgbaFloat color, Vector2 uv)
        {
            Position = position;
            Color = color;
            Uv = uv;
            HighLighted = 0;
        }
    }
}
