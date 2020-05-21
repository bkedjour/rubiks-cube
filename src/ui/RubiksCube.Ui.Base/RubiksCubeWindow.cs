using System;
using System.Diagnostics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;

namespace RubiksCube.Ui.Base
{
    public class RubiksCubeWindow : IWindow
    {
        public event Action<float> Rendering;
        public event Action<GraphicsDevice, ResourceFactory, Swapchain> GraphicsDeviceCreated;
        public event Action GraphicsDeviceDestroyed;
        public event Action Resized;
        public event Action<KeyEvent> KeyPressed;

        private readonly Sdl2Window _window;
        private GraphicsDevice _graphicsDevice;
        private DisposeCollectorResourceFactory _factory;
        private bool _windowResized = true;

        public uint Width => (uint)_window.Width;
        public uint Height => (uint)_window.Height;

        public RubiksCubeWindow(string title)
        {
            var wci = new WindowCreateInfo
            {
                X = 100,
                Y = 100,
                WindowWidth = 1280,
                WindowHeight = 720,
                WindowTitle = title,
            };
            
            _window = VeldridStartup.CreateWindow(ref wci);
            
            _window.Resized += () =>
            {
                _windowResized = true;
            };
            
            _window.KeyDown += OnKeyDown;
        }
        public void Run()
        {
            var options = new GraphicsDeviceOptions(false, PixelFormat.R16_UNorm, true, ResourceBindingModel.Improved,
                true, true);
#if DEBUG
            options.Debug = true;
#endif
            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(_window, options);
            _factory = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            GraphicsDeviceCreated?.Invoke(_graphicsDevice, _factory, _graphicsDevice.MainSwapchain);

            var sw = Stopwatch.StartNew();
            var previousElapsed = sw.Elapsed.TotalSeconds;

            while (_window.Exists)
            {
                var newElapsed = sw.Elapsed.TotalSeconds;
                var deltaSeconds = (float)(newElapsed - previousElapsed);

                InputSnapshot inputSnapshot = _window.PumpEvents();
                InputTracker.UpdateFrameInput(inputSnapshot);

                if (!_window.Exists) continue;

                previousElapsed = newElapsed;
                if (_windowResized)
                {
                    _windowResized = false;
                    _graphicsDevice.ResizeMainWindow((uint)_window.Width, (uint)_window.Height);
                    Resized?.Invoke();
                }

                Rendering?.Invoke(deltaSeconds);
            }

            _graphicsDevice.WaitForIdle();
            _factory.DisposeCollector.DisposeAll();
            _graphicsDevice.Dispose();
            GraphicsDeviceDestroyed?.Invoke();
        }

        protected void OnKeyDown(KeyEvent keyEvent)
        {
            KeyPressed?.Invoke(keyEvent);
        }
    }
}
