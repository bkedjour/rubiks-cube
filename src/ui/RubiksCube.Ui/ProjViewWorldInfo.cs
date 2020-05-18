using System.Numerics;

namespace RubiksCube.Ui
{
    public struct ProjViewWorldInfo
    {
        public Matrix4x4 Projection;
        public Matrix4x4 View;
        public Matrix4x4 World;
    }
}
