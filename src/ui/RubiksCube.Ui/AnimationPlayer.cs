using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RubiksCube.Engine;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Ui
{
    public class AnimationPlayer
    {
        public bool AnimationInProgress => _animations.Count > 0;
        
        private readonly List<Animation> _animations = new List<Animation>();

        public void Play(Matrix4x4 initialRotation, RotationInfo rotation, CellDecorator cell)
        {
            _animations.Add(new Animation(initialRotation, rotation, cell));
        }

        public void Update(float deltaSeconds)
        {
            if (!AnimationInProgress)
                return;

            var finishedAnimations = _animations.Where(animation =>
                Math.Abs(animation.Angle - animation.TargetRotation.Angle) < float.Epsilon).ToList();

            finishedAnimations.ForEach(a => _animations.Remove(a));

            foreach (var animation in _animations)
            {
                animation.Angle += deltaSeconds * 200 * (animation.TargetRotation.Angle < 0 ? -1 : 1);
                if (Math.Abs(animation.Angle) > Math.Abs(animation.Cell.Cell.RotationInfo.Angle))
                    animation.Angle = animation.Cell.Cell.RotationInfo.Angle;

                animation.Cell.AnimationRotation = animation.InitialRotation * animation.TargetRotation.Axis switch
                {
                    Axis.X => Matrix4x4.CreateRotationX(animation.Angle.ToRadians()),
                    Axis.Y => Matrix4x4.CreateRotationY(animation.Angle.ToRadians()),
                    Axis.Z => Matrix4x4.CreateRotationZ(animation.Angle.ToRadians()),
                    Axis.None => Matrix4x4.Identity,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private class Animation
        {
            public Animation(Matrix4x4 initialRotation, RotationInfo targetRotation, CellDecorator cell)
            {
                InitialRotation = initialRotation;
                TargetRotation = targetRotation;
                Cell = cell;
            }

            public Matrix4x4 InitialRotation { get; }
            public RotationInfo TargetRotation { get; }
            public CellDecorator Cell { get; }
            public float Angle { get; set; }
        }
    }
}
