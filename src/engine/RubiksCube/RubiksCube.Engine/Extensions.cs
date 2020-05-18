using System;

namespace RubiksCube.Engine
{
    public static class Extensions
    {
        public static float ToRadians(this float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }

        public static float ToRadians(this int degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }
    }
}
