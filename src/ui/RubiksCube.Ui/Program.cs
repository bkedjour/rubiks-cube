using RubiksCube.Ui.Base;

namespace RubiksCube.Ui
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new RubiksCubeWindow("Rubiks Cube");
            var app = new RubiksCubeApplication(window);
            window.Run();
        }
    }
}
