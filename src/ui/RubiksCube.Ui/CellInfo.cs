using System;
using System.Numerics;
using RubiksCube.Engine;
using Veldrid;

namespace RubiksCube.Ui
{
    public class CellInfo : Component, IDisposable
    {
        public Cell Cell { get; }

        public Vector3 Translation { get; }
        public Matrix4x4 Rotation { get; set; }

        
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;

        private Shader[] _shaders;
        private Pipeline _pipeline;

        private DeviceBuffer _projViewWorldBuffer;
        private ResourceSet _projViewWorldSet;

        private VertexPositionColor[] _vertices;
        private ushort[] _indices;

        public CellInfo(Cell cell, Vector3 translation, ResourceFactory factory, GraphicsDevice graphicsDevice,
            CommandList commandList, Shader[] shaders) : base(graphicsDevice,factory,commandList)
        {
            Cell = cell;
            Translation = translation;
            _shaders = shaders;

            CreateResources();
        }

        private void CreateResources()
        {
            _vertexBuffer = Factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            _indexBuffer = Factory.CreateBuffer(new BufferDescription(6 * sizeof(ushort), BufferUsage.IndexBuffer));
            _projViewWorldBuffer = Factory.CreateBuffer(new BufferDescription(3 * 64, BufferUsage.UniformBuffer));

            CreateVertices();

            GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, _vertices);
            GraphicsDevice.UpdateBuffer(_indexBuffer, 0, _indices);

            var projViewWorldLayout = Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionViewWorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            var pipelineDescription = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerState = RasterizerStateDescription.Default,
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ResourceLayouts = new[] { projViewWorldLayout },
                ShaderSet = new ShaderSetDescription(
                    new[]
                    {
                        new VertexLayoutDescription(
                            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                                VertexElementFormat.Float3),
                            new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate,
                                VertexElementFormat.Float4))
                    }, _shaders),
                Outputs = GraphicsDevice.SwapchainFramebuffer.OutputDescription
            };
           _pipeline = Factory.CreateGraphicsPipeline(pipelineDescription);

            _projViewWorldSet = Factory.CreateResourceSet(new ResourceSetDescription(projViewWorldLayout, _projViewWorldBuffer));
        }

        private void CreateVertices()
        {
            _vertices = new[]
            {
                new VertexPositionColor(new Vector3(-.5f, .5f, .5f), GetRgbaColor(Cell.Color)),
                new VertexPositionColor(new Vector3(.5f, .5f, .5f), GetRgbaColor(Cell.Color)),
                new VertexPositionColor(new Vector3(.5f, -.5f, .5f), GetRgbaColor(Cell.Color)),
                new VertexPositionColor(new Vector3(-.5f, -.5f, .5f), GetRgbaColor(Cell.Color))
            };

            _indices = new ushort[]
            {
                0, 1, 2, 0, 2, 3
            };
        }

        private RgbaFloat GetRgbaColor(Color color)
        {
            switch (color)
            {
                case Color.Yellow:
                    return RgbaFloat.Yellow;
                case Color.White:
                    return RgbaFloat.White;
                case Color.Green:
                    return RgbaFloat.Green;
                case Color.Blue:
                    return RgbaFloat.Blue;
                case Color.Orange:
                    return RgbaFloat.Orange;
                case Color.Red:
                    return RgbaFloat.DarkRed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }

        public override void Update(float deltaSeconds, Matrix4x4 projection, Matrix4x4 view)
        {
            base.Update(deltaSeconds, projection, view);

            ProjViewWorld.World = Matrix4x4.CreateTranslation(Translation) * Rotation;

            CommandList.UpdateBuffer(_projViewWorldBuffer, 0, ref ProjViewWorld);
        }

        public void Draw(float deltaSeconds)
        {
            CommandList.SetPipeline(_pipeline);

            CommandList.SetVertexBuffer(0, _vertexBuffer);
            CommandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            CommandList.SetGraphicsResourceSet(0, _projViewWorldSet);

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
