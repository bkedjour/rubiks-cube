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

        protected ImGuiRenderer GuiRenderer;


        protected ApplicationBase(IWindow window)
        {
            Window = window;
            Window.Resized += HandleWindowResize;
            Window.GraphicsDeviceCreated += OnGraphicsDeviceCreated;
            Window.GraphicsDeviceDestroyed += OnDeviceDestroyed;
            Window.Rendering += Update;
            window.Rendering += (deltaSeconds, inputSnapshot) => Draw(deltaSeconds);
            Window.KeyPressed += OnKeyDown;

            Camera = new Camera(Window.Width, Window.Height);
        }

        protected virtual void HandleWindowResize()
        {
            Camera.WindowResized(Window.Width, Window.Height);
            GuiRenderer.WindowResized((int) Window.Width, (int) Window.Height);
        }

        public void OnGraphicsDeviceCreated(GraphicsDevice graphicsDevice, ResourceFactory factory, Swapchain swapchain)
        {
            GraphicsDevice = graphicsDevice;
            ResourceFactory = factory;
            MainSwapchain = swapchain;

            GuiRenderer = new ImGuiRenderer(graphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
                (int)Window.Width, (int) Window.Height);

            CreateResources(factory);
            CreateSwapchainResources(factory);
        }

        protected virtual void OnDeviceDestroyed()
        {
            GraphicsDevice = null;
            ResourceFactory = null;
            MainSwapchain = null;
        }

        protected virtual void Update(float deltaSeconds, InputSnapshot inputSnapshot)
        {
            Camera.Update(deltaSeconds);
            GuiRenderer.Update(deltaSeconds, inputSnapshot);
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
