using System;
using System.Numerics;
using RubiksCube.Ui.Base;
using Veldrid;
using Veldrid.SPIRV;

namespace RubiksCube.Ui
{
    public class RubiksCubeApplication : ApplicationBase
    {
        private static CommandList _commandList;
        private static DeviceBuffer _vertexBuffer;
        private static DeviceBuffer _indexBuffer;
        
        private static Shader[] _shaders;
        private static Pipeline _pipeline;

        private DeviceBuffer _projViewWorldBuffer;
        private ResourceSet _projViewWorldSet;

        private VertexPositionColor[] _quadVertices;
        private ushort[] _quadIndices;

        private float _ticks;

        public RubiksCubeApplication(IWindow window) : base(window)
        {
            Camera.Position = new Vector3(0, 0, 1f);
        }

        protected override void CreateResources(ResourceFactory factory)
        {
            _vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            _indexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));
            _projViewWorldBuffer = factory.CreateBuffer(new BufferDescription(3 * 64, BufferUsage.UniformBuffer));

            CreateRectangle();

            GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, _quadVertices);
            GraphicsDevice.UpdateBuffer(_indexBuffer, 0, _quadIndices);

            var projViewWorldLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionViewWorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));


            var pipelineDescription = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerState = RasterizerStateDescription.Default,
                PrimitiveTopology = PrimitiveTopology.TriangleStrip,
                ResourceLayouts = new []{projViewWorldLayout},
                ShaderSet = LoadShaders(factory),
                Outputs = GraphicsDevice.SwapchainFramebuffer.OutputDescription
            };

            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            _projViewWorldSet = factory.CreateResourceSet(new ResourceSetDescription(projViewWorldLayout,_projViewWorldBuffer));
            
            _commandList = factory.CreateCommandList();
        }

        private void CreateRectangle()
        {
            _quadVertices = new[]
            {
                new VertexPositionColor(new Vector3(-.75f, .75f, -1f), RgbaFloat.Red),
                new VertexPositionColor(new Vector3(.75f, .75f, -1f), RgbaFloat.Green),
                new VertexPositionColor(new Vector3(-.75f, -.75f, -1f), RgbaFloat.Blue),
                new VertexPositionColor(new Vector3(.75f, -.75f, -1f), RgbaFloat.Yellow)
            };

            _quadIndices = new ushort[] {0, 1, 2, 3};
        }

        private ShaderSetDescription LoadShaders(ResourceFactory factory)
        {
            _shaders = factory.CreateFromSpirv(
                new ShaderDescription(ShaderStages.Vertex, ReadEmbeddedAssetBytes("Vertex.glsl"), "main"),
                new ShaderDescription(ShaderStages.Fragment, ReadEmbeddedAssetBytes("Fragment.glsl"), "main"));

            return new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float3),
                        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate,
                            VertexElementFormat.Float4))
                }, _shaders);
        }


        protected override void Draw(float deltaSeconds)
        {
            _ticks += deltaSeconds;

            UpdateUniformBuffers();

            _commandList.Begin();

            

            _commandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            _commandList.ClearColorTarget(0, RgbaFloat.Black);
            _commandList.ClearDepthStencil(1f);
            _commandList.SetPipeline(_pipeline);

            _commandList.SetVertexBuffer(0, _vertexBuffer);
            _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _commandList.SetGraphicsResourceSet(0, _projViewWorldSet);

            _commandList.DrawIndexed(4, 1, 0, 0, 0);

            _commandList.End();
            
            GraphicsDevice.SubmitCommands(_commandList);
            GraphicsDevice.SwapBuffers();
        }

        private void UpdateUniformBuffers()
        {
            var projViewWorldInfo = new ProjViewWorldInfo
            {
                Projection = Matrix4x4.CreatePerspectiveFieldOfView(1.0f, (float)Window.Width / Window.Height, 0.5f, 100f),
                View = Matrix4x4.CreateLookAt(Camera.Position, Camera.Position + Camera.Forward, Vector3.UnitY),
                World = Matrix4x4.Identity
            };
            
            _commandList.UpdateBuffer(_projViewWorldBuffer, 0, ref projViewWorldInfo);
        }

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180f;
        }

        protected override void OnDeviceDestroyed()
        {
            _pipeline.Dispose();

            foreach (var shader in _shaders)
                shader.Dispose();

            _commandList.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();

            base.OnDeviceDestroyed();
        }
    }
}
