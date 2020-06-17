using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using ImGuiNET;
using RubiksCube.Engine;
using RubiksCube.Engine.Enums;
using RubiksCube.Manipulation;
using RubiksCube.Ui.Base;
using Veldrid;
using Veldrid.SPIRV;

namespace RubiksCube.Ui
{
    public class RubiksCubeApplication : ApplicationBase
    {
        private static CommandList _commandList;
        private static Shader[] _shaders;

        private readonly Cube _cube;
        private List<CellDecorator> _cellsDecorators;

        private Vector2 _previousMousePos;
        private float _yaw;
        private float _pitch;

        private readonly AnimationPlayer _animationPlayer;

        private readonly Shuffler _shuffler;

        public RubiksCubeApplication(IWindow window) : base(window)
        {
            var cubeFactory = new CubeFactory();
            _cube = (Cube) cubeFactory.CreateCube();
            _animationPlayer = new AnimationPlayer();

            _shuffler = new Shuffler(_cube);
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

            var processor = new ImageSharpProcessor();
            ProcessedTexture processedTexture;
            using (var fs = File.OpenRead(@"assets\cell.png"))
            {
                processedTexture = processor.Process(fs);
            }

            var texture = processedTexture.CreateDeviceTexture(GraphicsDevice, factory, TextureUsage.Sampled);

            using (var fs = File.OpenRead(@"assets\highLighted-cell.png"))
            {
                processedTexture = processor.Process(fs);
            }

            var highLightedTexture =
                processedTexture.CreateDeviceTexture(GraphicsDevice, factory, TextureUsage.Sampled);
            var textures = new[] {texture, highLightedTexture};

            CreateFace(Side.Front, factory, textures);
            CreateFace(Side.Up, factory, textures);
            CreateFace(Side.Down, factory, textures);
            CreateFace(Side.Right, factory, textures);
            CreateFace(Side.Back, factory, textures);
            CreateFace(Side.Left, factory, textures);
        }

        private void CreateFace(Side side, ResourceFactory factory, Texture[] textures)
        {
            var cells = _cube.GetFace(side).Cells;
            var index = 0;
            
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    _cellsDecorators.Add(new CellDecorator(cells[index++],
                        new Vector3((row - 1), -(col - 1), 1),
                        factory, GraphicsDevice, _commandList, _shaders, _animationPlayer, textures));
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

            HandleCubeMoves(deltaSeconds);
            HandleUserInput();

            if (InputTracker.GetMouseButton(MouseButton.Left))
            {
                _yaw += mouseDelta.X * 0.007f;
                _pitch += mouseDelta.Y * 0.007f;
            }

            _animationPlayer.Update(deltaSeconds);

            foreach (var cell in _cellsDecorators)
            {
                cell.Rotation = Matrix4x4.CreateRotationY(_yaw) * Matrix4x4.CreateRotationX(_pitch);
                cell.Update(deltaSeconds, projection, view);
            }

        }

        private int _currentCubeMoveIndex = 0;
        private void HandleCubeMoves(float deltaSeconds)
        {
            if (_cube.HasNextMove && !_animationPlayer.AnimationInProgress)
            {
                _cube.PlayNextMove();
                _currentCubeMoveIndex++;
            }
        }

        private void HandleUserInput()
        {
            if (_animationPlayer.AnimationInProgress) return;

            if (InputTracker.GetKeyDown(Key.R))
                _cube.Move(Side.Right, Direction.Clockwise);

            if (InputTracker.GetKeyDown(Key.T))
                _cube.Move(Side.Right, Direction.Counterclockwise);

            if (InputTracker.GetKeyDown(Key.L))
                _cube.Move(Side.Left, Direction.Clockwise);

            if (InputTracker.GetKeyDown(Key.M))
                _cube.Move(Side.Left, Direction.Counterclockwise);

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

            if(InputTracker.GetKeyDown(Key.S))
                _shuffler.Shuffle();
        }

        private void DrawText(float deltaSeconds)
        {
            ImGui.Begin(string.Empty,
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove);
            ImGui.SetWindowSize(new Vector2(Window.Width, Window.Height));
            ImGui.SetWindowPos(Vector2.Zero);

            ImGui.Text($"Fps: {Math.Round(Window.Fps)}");
            ImGui.Text("Faces: R L U D F B");
            ImGui.Text("CUBE: X Y Z");
            ImGui.Text("Shuffle: S");
            ImGui.TextColored(new Vector4(255,0,0,1),$"Moves: {_currentCubeMoveIndex}/{_cube._moves.Count}" );

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
