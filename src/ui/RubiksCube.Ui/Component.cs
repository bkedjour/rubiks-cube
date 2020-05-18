using System.Numerics;
using Veldrid;

namespace RubiksCube.Ui
{
    public abstract class Component
    {
        protected readonly GraphicsDevice GraphicsDevice;
        protected readonly ResourceFactory Factory;
        protected readonly CommandList CommandList;

        protected ProjViewWorldInfo ProjViewWorld;

        protected Component(GraphicsDevice graphicsDevice, ResourceFactory factory, CommandList commandList)
        {
            GraphicsDevice = graphicsDevice;
            Factory = factory;
            CommandList = commandList;
        }

        public virtual void Update(float deltaSeconds, Matrix4x4 projection, Matrix4x4 view)
        {
            ProjViewWorld = new ProjViewWorldInfo
            {
                Projection = projection,
                View = view
            };
        }
    }
}
