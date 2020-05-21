using System;
using Veldrid;

namespace RubiksCube.Ui.Base
{
    public interface IWindow
    {
        event Action<float, InputSnapshot> Rendering;
        event Action<GraphicsDevice, ResourceFactory, Swapchain> GraphicsDeviceCreated;
        event Action GraphicsDeviceDestroyed;
        event Action Resized;
        event Action<KeyEvent> KeyPressed;

        uint Width { get; }
        uint Height { get; }

        void Run();
    }
}
