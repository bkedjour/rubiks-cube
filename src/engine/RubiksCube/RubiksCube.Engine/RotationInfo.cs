using System;
using System.Numerics;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Engine
{
    public class RotationInfo
    {
        public Axis Axis { get; set; }

        public float Angle { get; set; }

        public Matrix4x4 RotationMatrix { get; set; }

        public RotationInfo()
        {
            RotationMatrix = Matrix4x4.Identity;
            Axis = Axis.None;
        }

        public RotationInfo(Axis axis, float angle)
        {
            Axis = axis;
            Angle = angle;

            RotationMatrix = axis switch
            {
                Axis.X => Matrix4x4.CreateRotationX(Angle.ToRadians()),
                Axis.Y => Matrix4x4.CreateRotationY(Angle.ToRadians()),
                Axis.Z => Matrix4x4.CreateRotationZ(Angle.ToRadians()),
                Axis.None => Matrix4x4.Identity,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };
        }

        public RotationInfo Clone()
        {
            return (RotationInfo) this.MemberwiseClone();
        }
    }
}
