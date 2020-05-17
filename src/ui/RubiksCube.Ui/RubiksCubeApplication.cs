using System;
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

        private List<CellInfo> _cells;

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
            _cells = new List<CellInfo>();

            foreach (var face in _cube.Faces)
            {
                for (int row = 0; row < face.Cells.GetLength(0); row++)
                {
                    for (int col = 0; col < face.Cells.GetLength(1); col++)
                    {
                        _cells.Add(new CellInfo(face.Cells[row, col], new Vector3((row - 1) * 1.05f, (col - 1) * -1.05f, 1.1f), factory, GraphicsDevice,
                            _commandList, _shaders)
                        {
                            Rotation = GetRotation(face.Side)
                        });
                    }
                }
                
            }
        }

        private Matrix4x4 GetRotation(Side side)
        {
            switch (side)
            {
                case Side.Front:
                    return Matrix4x4.Identity;
                case Side.Back:
                    return Matrix4x4.CreateRotationY(DegreesToRadians(180));
                case Side.Right:
                    return Matrix4x4.CreateRotationY(DegreesToRadians(90));
                case Side.Left:
                    return Matrix4x4.CreateRotationY(DegreesToRadians(-90));
                case Side.Up:
                    return Matrix4x4.CreateRotationX(DegreesToRadians(-90));
                case Side.Down:
                    return Matrix4x4.CreateRotationX(DegreesToRadians(90));
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }

        private float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }

        protected override void Draw(float deltaSeconds)
        {
            var projection =
                Matrix4x4.CreatePerspectiveFieldOfView(1.0f, (float)Window.Width / Window.Height, 0.5f, 100f);

            var view = Matrix4x4.CreateLookAt(Camera.Position, Camera.Position + Camera.Forward, Vector3.UnitY);

            foreach (var cell in _cells)
            {
                cell.Rotation *= Matrix4x4.CreateRotationY(deltaSeconds) * Matrix4x4.CreateRotationX(deltaSeconds/2) * Matrix4x4.CreateRotationZ(deltaSeconds/4);
                cell.Update(deltaSeconds, projection, view);
            }

            _commandList.Begin();
            _commandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            _commandList.ClearColorTarget(0, RgbaFloat.Black);
            _commandList.ClearDepthStencil(1f);

            foreach (var cell in _cells)
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

            foreach (var cell in _cells)
                cell.Dispose();

            base.OnDeviceDestroyed();
        }
    }
}