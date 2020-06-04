using System;
using System.Numerics;
using RubiksCube.Engine;
using RubiksCube.Engine.Enums;
using Veldrid;

namespace RubiksCube.Ui
{
    public class CellDecorator : Component, IDisposable
    {
        public Cell Cell { get; }
        public Vector3 Translation { get; }
        public Matrix4x4 Rotation { get; set; }
        public Matrix4x4 AnimationRotation { get; set; }
        
        private Matrix4x4 _previousRotation;

        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;

        private readonly Shader[] _shaders;
        private Pipeline _pipeline;

        private DeviceBuffer _projViewWorldBuffer;
        private ResourceSet _projViewWorldSet;
        private Texture _texture;
        private ResourceSet _textureSet;

        private VertexInfo[] _vertices;
        private ushort[] _indices;

        private readonly AnimationPlayer _animationPlayer;

        public CellDecorator(Cell cell, Vector3 translation, ResourceFactory factory, GraphicsDevice graphicsDevice,
            CommandList commandList, Shader[] shaders, AnimationPlayer animationPlayer, Texture texture) : base(graphicsDevice, factory, commandList)
        {
            Cell = cell;
            _previousRotation = cell.RotationInfo.RotationMatrix;
            Translation = translation;
            _shaders = shaders;
            _animationPlayer = animationPlayer;
            _texture = texture;
            Rotation = Matrix4x4.Identity;
            CreateResources();
        }

        private void CreateResources()
        {
            _vertexBuffer = Factory.CreateBuffer(new BufferDescription(4 * VertexInfo.SizeInBytes, BufferUsage.VertexBuffer));
            _indexBuffer = Factory.CreateBuffer(new BufferDescription(6 * sizeof(ushort), BufferUsage.IndexBuffer));
            _projViewWorldBuffer = Factory.CreateBuffer(new BufferDescription(3 * 64, BufferUsage.UniformBuffer));

            CreateVertices();

            GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, _vertices);
            GraphicsDevice.UpdateBuffer(_indexBuffer, 0, _indices);

            var projViewWorldLayout = Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionViewWorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            var textureView = Factory.CreateTextureView(_texture);

            ResourceLayout textureLayout = Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            _textureSet = Factory.CreateResourceSet(new ResourceSetDescription(
                textureLayout, textureView, GraphicsDevice.Aniso4xSampler));

            var pipelineDescription = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerState = RasterizerStateDescription.Default,
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ResourceLayouts = new[] { projViewWorldLayout, textureLayout },
                ShaderSet = new ShaderSetDescription(
                    new[]
                    {
                        new VertexLayoutDescription(
                            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                            new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
                            new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
                    }, _shaders),
                Outputs = GraphicsDevice.SwapchainFramebuffer.OutputDescription
            };
           _pipeline = Factory.CreateGraphicsPipeline(pipelineDescription);

            _projViewWorldSet = Factory.CreateResourceSet(new ResourceSetDescription(projViewWorldLayout, _projViewWorldBuffer));
        }

        private void CreateVertices()
        {
            var color = GetRgbaColor(Cell.Color);
            _vertices = new[]
            {
                new VertexInfo(new Vector3(-.5f, .5f, .5f), color, new Vector2(0, 1)),
                new VertexInfo(new Vector3(.5f, .5f, .5f), color, new Vector2(1, 1)),
                new VertexInfo(new Vector3(.5f, -.5f, .5f), color, new Vector2(1, 0)),
                new VertexInfo(new Vector3(-.5f, -.5f, .5f), color, new Vector2(0, 0))
            };

            _indices = new ushort[]
            {
                0, 1, 2, 0, 2, 3
            };
        }

        private RgbaFloat GetRgbaColor(Color color)
        {
            return color switch
            {
                Color.Yellow => RgbaFloat.Yellow,
                Color.White => RgbaFloat.White,
                Color.Green => RgbaFloat.Green,
                Color.Blue => RgbaFloat.Blue,
                Color.Orange => RgbaFloat.Orange,
                Color.Red => RgbaFloat.DarkRed,
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
        }

        public override void Update(float deltaSeconds, Matrix4x4 projection, Matrix4x4 view)
        {
            base.Update(deltaSeconds, projection, view);

            if (_previousRotation != Cell.RotationInfo.RotationMatrix)
            {
                _animationPlayer.Play(_previousRotation, Cell.RotationInfo, this);
            }

            _previousRotation = Cell.RotationInfo.RotationMatrix;

            if (!_animationPlayer.AnimationInProgress) AnimationRotation = Cell.RotationInfo.RotationMatrix;
            
            ProjViewWorld.World = Matrix4x4.CreateTranslation(Translation) * AnimationRotation * Rotation;

            CommandList.UpdateBuffer(_projViewWorldBuffer, 0, ref ProjViewWorld);
        }

        public void Draw(float deltaSeconds)
        {
            CommandList.SetPipeline(_pipeline);

            CommandList.SetVertexBuffer(0, _vertexBuffer);
            CommandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            CommandList.SetGraphicsResourceSet(0, _projViewWorldSet);
            CommandList.SetGraphicsResourceSet(1, _textureSet);

            CommandList.DrawIndexed(6, 1, 0, 0, 0);
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _pipeline.Dispose();
        }
    }
}
