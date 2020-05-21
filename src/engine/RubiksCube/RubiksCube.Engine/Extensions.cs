using System;
using System.Numerics;

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
            return ToRadians((float) degrees);
        }

        public static Vector3 Round(this Vector3 vector)
        {
            return new Vector3((float) Math.Round(vector.X), (float) Math.Round(vector.Y),
                (float) Math.Round(vector.Z));
        }
    }
}
