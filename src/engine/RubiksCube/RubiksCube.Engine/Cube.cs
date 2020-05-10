using System.Collections.Generic;
using System.Linq;

namespace RubiksCube.Engine
{
    public class Cube : ICube
    {
        public IEnumerable<Face> Faces { get; }

        public Status Status => Faces.All(f => f.Status == Status.Solved) ? Status.Solved : Status.NotSolved;

        public Cube(IEnumerable<Face> faces)
        {
            Faces = faces;
        }


        public void Shuffle()
        {
            throw new System.NotImplementedException();
        }

        public void Rotate(Axis axis, int angle)
        {
            throw new System.NotImplementedException();
        }

        public void Move(Side side, Direction direction)
        {
            throw new System.NotImplementedException();
        }
    }
}
