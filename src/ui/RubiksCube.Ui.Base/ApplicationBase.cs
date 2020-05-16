using System.IO;
using Veldrid;

namespace RubiksCube.Ui.Base
{
    public abstract class ApplicationBase
    {
        public IWindow Window { get; }
        public GraphicsDevice GraphicsDevice { get; private set; }
        public ResourceFactory ResourceFactory { get; private set; }
        public Swapchain MainSwapchain { get; private set; }

        protected Camera Camera;

        protected ApplicationBase(IWindow window)
        {
            Window = window;
            Window.Resized += HandleWindowResize;
            Window.GraphicsDeviceCreated += OnGraphicsDeviceCreated;
            Window.GraphicsDeviceDestroyed += OnDeviceDestroyed;
            Window.Rendering += PreDraw;
            Window.Rendering += Draw;
            Window.KeyPressed += OnKeyDown;

            Camera = new Camera(Window.Width, Window.Height);
        }

        protected virtual void HandleWindowResize()
        {
            Camera.WindowResized(Window.Width, Window.Height);
        }

        public void OnGraphicsDeviceCreated(GraphicsDevice graphicsDevice, ResourceFactory factory, Swapchain swapchain)
        {
            GraphicsDevice = graphicsDevice;
            ResourceFactory = factory;
            MainSwapchain = swapchain;
            CreateResources(factory);
            CreateSwapchainResources(factory);
        }

        protected virtual void OnDeviceDestroyed()
        {
            GraphicsDevice = null;
            ResourceFactory = null;
            MainSwapchain = null;
        }

        private void PreDraw(float deltaSeconds)
        {
            Camera.Update(deltaSeconds);
        }

        protected abstract void CreateResources(ResourceFactory factory);
        
        protected virtual void CreateSwapchainResources(ResourceFactory factory) { }

        protected abstract void Draw(float deltaSeconds);

        protected virtual void OnKeyDown(KeyEvent keyEvent) { }
        
        public Stream OpenEmbeddedAssetStream(string name) => GetType().Assembly.GetManifestResourceStream(name);
        
        public byte[] ReadEmbeddedAssetBytes(string name)
        {
            using (var stream = OpenEmbeddedAssetStream(name))
            {
                var bytes = new byte[stream.Length];
                using (var ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }
    }
}
