using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public class CubeMove
    {
        public Side Side { get; }

        public RotationInfo RotationInfo { get; }

        public CubeMove(Side side, RotationInfo rotationInfo)
        {
            Side = side;
            RotationInfo = rotationInfo;
        }

        public CubeMove(RotationInfo rotationInfo)
        {
            Side = (Side) (-1);
            RotationInfo = rotationInfo;
        }
    }
}
