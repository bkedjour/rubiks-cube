using System.Collections.Generic;
using System.Numerics;
using RubiksCube.Engine;
using RubiksCube.Ui.Base;
using Veldrid;
using Veldrid.SPIRV;

namespace RubiksCube.Ui
{
    public class RubiksCubeApplication : ApplicationBase
    {
        private static CommandList _commandList;
        private static Shader[] _shaders;

        private readonly ICube _cube;

        private List<CellDecorator> _cellsDecorators;

        public RubiksCubeApplication(IWindow window) : base(window)
        {
            Camera.Position = new Vector3(0, 0, 5f);
            
            var cubeFactory = new CubeFactory();
            _cube = cubeFactory.CreateCube();
        }

        protected override void CreateResources(ResourceFactory factory)
        {
            _shaders = factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, ReadEmbeddedAssetBytes("Vertex.glsl"), "main"),
                new ShaderDescription(ShaderStages.Fragment, ReadEmbeddedAssetBytes("Fragment.glsl"), "main"));

            _commandList = factory.CreateCommandList();

            InitializeCube(factory);
        }

        private void InitializeCube(ResourceFactory factory)
        {
            _cellsDecorators = new List<CellDecorator>();

            CreateFace(Side.Front, factory);
            CreateFace(Side.Up, factory);
            CreateFace(Side.Down, factory);
            CreateFace(Side.Right, factory);
            CreateFace(Side.Back, factory);
            CreateFace(Side.Left, factory);
        }

        private void CreateFace(Side side, ResourceFactory factory)
        {
            var cells = _cube.GetFace(side).Cells;
            int index = 0;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    _cellsDecorators.Add(new CellDecorator(cells[index], new Vector3((row - 1) * 1.05f, (col - 1) * -1.05f, 1.1f),
                        factory, GraphicsDevice, _commandList, _shaders));

                    index++;
                }
            }
        }

        private Direction direction = Direction.Clockwise;
        protected override void Update(float deltaSeconds)
        {
            base.Update(deltaSeconds);

            var projection =
                Matrix4x4.CreatePerspectiveFieldOfView(1.0f, (float)Window.Width / Window.Height, 0.5f, 100f);

            var view = Matrix4x4.CreateLookAt(Camera.Position, Camera.Position + Camera.Forward, Vector3.UnitY);

            if (InputTracker.GetKeyDown(Key.C))
            {
                direction = direction == Direction.Clockwise ? Direction.Counterclockwise : Direction.Clockwise;
            }

            if (InputTracker.GetKeyDown(Key.R))
            {
                _cube.Move(Side.Right, direction);
            }
            if (InputTracker.GetKeyDown(Key.L))
            {
                _cube.Move(Side.Left, direction);
            }
            if (InputTracker.GetKeyDown(Key.F))
            {
                _cube.Move(Side.Front, direction);
            }
            if (InputTracker.GetKeyDown(Key.B))
            {
                _cube.Move(Side.Back, direction);
            }
            if (InputTracker.GetKeyDown(Key.U))
            {
                _cube.Move(Side.Up, direction);
            }
            if (InputTracker.GetKeyDown(Key.G))
            {
                _cube.Move(Side.Down, direction);
            }

            if (InputTracker.GetKeyDown(Key.O))
            {
                _cube.Rotate(Axis.Y, 90);
            }

            foreach (var cell in _cellsDecorators)
            {
                //cell.Rotation *= Matrix4x4.CreateRotationY(deltaSeconds) * Matrix4x4.CreateRotationX(deltaSeconds/2) * Matrix4x4.CreateRotationZ(deltaSeconds/4);
                cell.Update(deltaSeconds, projection, view);
            }

        }

        protected override void Draw(float deltaSeconds)
        {
            _commandList.Begin();
            _commandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            _commandList.ClearColorTarget(0, RgbaFloat.Black);
            _commandList.ClearDepthStencil(1f);

            foreach (var cell in _cellsDecorators)
                cell.Draw(deltaSeconds);

            _commandList.End();

            GraphicsDevice.SubmitCommands(_commandList);
            GraphicsDevice.SwapBuffers();
        }

        protected override void OnDeviceDestroyed()
        {
            foreach (var shader in _shaders)
                shader.Dispose();

            _commandList.Dispose();

            foreach (var cell in _cellsDecorators)
                cell.Dispose();

            base.OnDeviceDestroyed();
        }
    }
}
