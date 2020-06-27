using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private Cube _cube;
        private List<CellDecorator> _cellsDecorators;

        private Vector2 _previousMousePos;
        private float _yaw;
        private float _pitch;

        private readonly AnimationPlayer _animationPlayer;

        private readonly Shuffler _shuffler;
        private readonly ISolver _solver;

        private bool _playAllMoves;

        public RubiksCubeApplication(IWindow window) : base(window)
        {
            var cubeFactory = new CubeFactory();
            _cube = (Cube) cubeFactory.CreateCube();
            _animationPlayer = new AnimationPlayer();

            _shuffler = new Shuffler(_cube);
            _shuffler.Shuffle();
            while (_cube.HasNextMove)
                _cube.PlayNextMove();
            _currentCubeMoveIndex = _cube.GetMoves().Count;
            _solver = new Cfop();
        }

        protected override void CreateResources(ResourceFactory factory)
        {
            _shaders = factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, ReadEmbeddedAssetBytes("Vertex.glsl"), "main"),
                new ShaderDescription(ShaderStages.Fragment, ReadEmbeddedAssetBytes("Fragment.glsl"), "main"));

            _commandList = factory.CreateCommandList();

            InitializeCube();
        }

        private void InitializeCube()
        {
            _cellsDecorators = new List<CellDecorator>();

            var processor = new ImageSharpProcessor();
            ProcessedTexture processedTexture;
            using (var fs = File.OpenRead(@"assets\cell.png"))
            {
                processedTexture = processor.Process(fs);
            }

            var texture = processedTexture.CreateDeviceTexture(GraphicsDevice, ResourceFactory, TextureUsage.Sampled);

            using (var fs = File.OpenRead(@"assets\highLighted-cell.png"))
            {
                processedTexture = processor.Process(fs);
            }

            var highLightedTexture = processedTexture.CreateDeviceTexture(GraphicsDevice, ResourceFactory, TextureUsage.Sampled);
            var textures = new[] {texture, highLightedTexture};

            CreateFace(Side.Front, textures);
            CreateFace(Side.Up, textures);
            CreateFace(Side.Down, textures);
            CreateFace(Side.Right, textures);
            CreateFace(Side.Back, textures);
            CreateFace(Side.Left, textures);
        }

        private void CreateFace(Side side, Texture[] textures)
        {
            var cells = _cube.GetFace(side).Cells;
            var index = 0;
            
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    var cell = cells[index++];
                    _cellsDecorators.Add(new CellDecorator(cell,
                        cell.InitialPosition,
                        //new Vector3((row - 1), -(col - 1), 1),
                        ResourceFactory, GraphicsDevice, _commandList, _shaders, _animationPlayer, textures));
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

            if (_playAllMoves)
                HandleCubeMoves();

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

        private int _currentCubeMoveIndex;
        private void HandleCubeMoves()
        {
            if (!_cube.HasNextMove)
                _playAllMoves = false;

            if (!_cube.HasNextMove || _animationPlayer.AnimationInProgress) return;

            _cube.PlayNextMove();
            _currentCubeMoveIndex++;
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
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.176f, 0.584f, 0.419f, 1));
            if (ImGui.Button("Shuffle", new Vector2(70, 25)))
            {
                _shuffler.Shuffle();
            }

            ImGui.PopStyleColor();

            if (ImGui.Button("Solve", new Vector2(70, 25)))
            {
                var moves = _solver.Solve(_cube);
                foreach (var cubeMove in moves)
                {
                    _cube.Move(cubeMove);
                }
            }

            if (ImGui.Button(">", new Vector2(70, 25)))
            {
                HandleCubeMoves();
            }

            if (ImGui.Button(">>", new Vector2(70, 25)))
            {
                _playAllMoves = true;
            }
            if (ImGui.Button(">>>", new Vector2(70, 25)))
            {
                while (_cube.HasNextMove)
                {
                    _cube.PlayNextMove();
                }
            }

            if (ImGui.Button("Simulate", new Vector2(70, 25)))
            {
                _movesStats.Clear();

                for (int i = 0; i < 10; i++)
                {
                    _shuffler.Shuffle();
                    while (_cube.HasNextMove)
                    {
                        HandleCubeMoves();
                    }

                    var clonedCube = _cube.Clone();
                    var moves = _solver.Solve(_cube);
                    foreach (var cubeMove in moves)
                    {
                        _cube.Move(cubeMove);
                        _cube.PlayNextMove();
                    }

                    _movesStats.Add(new Tuple<int, ICube, bool>(moves.Count, clonedCube, IsCubeOk(_cube)));
                }

                _movesStats.Sort((tuple, tuple1) => tuple.Item1.CompareTo(tuple1.Item1));
            }

            ImGui.TextColored(new Vector4(255, 0, 0, 1), $"Moves: {_currentCubeMoveIndex}/{_cube.GetMoves().Count}");


            ImGui.TextColored(new Vector4(0, 255, 255, 1), $"Average: {(_movesStats.Any() ? _movesStats.Average( s=> s.Item1):0)}");
            ImGui.TextColored(new Vector4(0, 255, 255, 1), $"Faults: {_movesStats.Count(c => !c.Item3)}");
            

            for (int i = 0; i < _movesStats.Count; i++)
            {
                if (!_movesStats[i].Item3)
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.176f, 0f, 0f, 1));

                if (ImGui.Button($"{_movesStats[i].Item1}", new Vector2(70, 25)))
                {
                    _cube = (Cube) _movesStats[i].Item2.Clone();
                    InitializeCube();
                    _currentCubeMoveIndex = 0;
                }

                if (!_movesStats[i].Item3)
                    ImGui.PopStyleColor();
            }

            ImGui.End();
            GuiRenderer.Render(GraphicsDevice, _commandList);
        }
        
        private List<Tuple<int, ICube, bool>> _movesStats = new List<Tuple<int, ICube,bool>>();

        bool IsCubeOk(ICube cube)
        {
            var center = cube.GetFace(Side.Front).Cells.Single(c => c.Position == new Vector3(0, 0, 1));
            if (cube.Cells.Single(c => c.Position == new Vector3(0, -1, 1) && c.Normal.Y == -1).Color != Color.White)
                return false;

            if (cube.Cells.Single(c => c.Position == new Vector3(0, -1, 1) && c.Normal.Z == 1).Color != Color.Green)
                return false;

            if (cube.Cells.Single(c => c.Position == new Vector3(1, -1, 0) && c.Normal.Y == -1).Color != Color.White)
                return false;

            if (cube.Cells.Single(c => c.Position == new Vector3(1, -1, 0) && c.Normal.X == 1).Color != Color.Orange)
                return false;

            if (cube.Cells.Single(c => c.Position == new Vector3(0, -1, -1) && c.Normal.Y == -1).Color != Color.White)
                return false;

            if (cube.Cells.Single(c => c.Position == new Vector3(0, -1, -1) && c.Normal.Z == -1).Color != Color.Blue)
                return false;

            if (cube.Cells.Single(c => c.Position == new Vector3(-1, -1, 0) && c.Normal.Y == -1).Color != Color.White)
                return false;

            if (cube.Cells.Single(c => c.Position == new Vector3(-1, -1, 0) && c.Normal.X == -1).Color != Color.Red)
                return false;
            return true;
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
