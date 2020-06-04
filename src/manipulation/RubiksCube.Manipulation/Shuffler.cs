using System;
using RubiksCube.Engine;
using RubiksCube.Engine.Enums;

namespace RubiksCube.Manipulation
{
    public class Shuffler
    {
        private readonly ICube _cube;
        private readonly Random _random;

        private const int Moves = 25;

        public Shuffler(ICube cube)
        {
            _cube = cube;
            _random = new Random();
        }

        public void Shuffle()
        {
            for (var i = 0; i < Moves; i++)
            {
               var side = _random.Next(0, 6);
               var direction = _random.Next(0, 2);
               _cube.Move((Side) side, (Direction) direction);
            }
        }
    }
}
