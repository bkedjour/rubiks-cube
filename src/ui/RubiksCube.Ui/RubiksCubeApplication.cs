using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using RubiksCube.Engine;
using RubiksCube.Engine.Enums;
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

        private Vector2 _previousMousePos;
        private float _yaw;
        private float _pitch;

        public RubiksCubeApplication(IWindow window) : base(window)
        {
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
            var index = 0;
            
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    _cellsDecorators.Add(new CellDecorator(cells[index++], new Vector3((row - 1) * 1.02f, (col - 1) * -1.02f, 1.04f),
                        factory, GraphicsDevice, _commandList, _shaders));
                }
            }
        }

        

        protected override void Update(float deltaSeconds, InputSnapshot inputSnapshot)
        {
            base.Update(deltaSeconds, inputSnapshot);

            var projection =
                Matrix4x4.CreatePerspectiveFieldOfView(1.0f, (float)Window.Width / Window.Height, 0.5f, 100f);

            var view = Matrix4x4.CreateLookAt(new Vector3(3.3f, 3, 4.4f), Vector3.Zero, Vector3.UnitY);

            Vector2 mouseDelta = InputTracker.MousePosition - _previousMousePos;
            _previousMousePos = InputTracker.MousePosition;

            HandleUserInput();

            if (InputTracker.GetMouseButton(MouseButton.Left))
            {
                _yaw += mouseDelta.X * 0.007f;
                _pitch += mouseDelta.Y * 0.007f;
            }

            foreach (var cell in _cellsDecorators)
            {
                cell.Rotation = Matrix4x4.CreateRotationY(_yaw) * Matrix4x4.CreateRotationX(_pitch);
                cell.Update(deltaSeconds, projection, view);
            }

        }

        private void HandleUserInput()
        {
            if (InputTracker.GetKeyDown(Key.R))
                _cube.Move(Side.Right, Direction.Clockwise);

            if (InputTracker.GetKeyDown(Key.L))
                _cube.Move(Side.Left, Direction.Clockwise);

            if (InputTracker.GetKeyDown(Key.U))
                _cube.Move(Side.Up, Direction.Clockwise);

            if (InputTracker.GetKeyDown(Key.D))
                _cube.Move(Side.Down, Direction.Clockwise);

            if (InputTracker.GetKeyDown(Key.F))
                _cube.Move(Side.Front, Direction.Clockwise);

            if (InputTracker.GetKeyDown(Key.B))
                _cube.Move(Side.Back, Direction.Clockwise);


            if (InputTracker.GetKeyDown(Key.X))
                _cube.Rotate(Axis.X, 90);

            if (InputTracker.GetKeyDown(Key.Y))
                _cube.Rotate(Axis.Y, 90);

            if (InputTracker.GetKeyDown(Key.Z))
                _cube.Rotate(Axis.Z, 90);
        }

        private void DrawText(float deltaSeconds)
        {
            ImGui.Begin(string.Empty,
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove);
            ImGui.SetWindowSize(new Vector2(250, Window.Height));
            ImGui.SetWindowPos(Vector2.Zero);

            ImGui.Text($"Fps: {Math.Round(Window.Fps)}");
            ImGui.Text("Faces: R L U D F B");
            ImGui.Text("CUBE: X Y Z");

            ImGui.End();
            GuiRenderer.Render(GraphicsDevice, _commandList);
        }

        protected override void Draw(float deltaSeconds)
        {

            _commandList.Begin();
            _commandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            _commandList.ClearColorTarget(0, RgbaFloat.Black);
            _commandList.ClearDepthStencil(1f);

            foreach (var cell in _cellsDecorators)
                cell.Draw(deltaSeconds);

            DrawText(deltaSeconds);

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
