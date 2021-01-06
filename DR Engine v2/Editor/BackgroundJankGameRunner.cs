using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Editor
{
    /// <summary>
    ///     Here because we can't initialize a GraphicsDevice on its own. This is dumb.
    /// </summary>
    public class BackgroundJankGameRunner : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;

        public BackgroundJankGameRunner()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false
            };
            // In initialize: new SpriteBatch(GraphicsDevice);?
        }

        public GraphicsDevice GraphicsDevice => _graphics.GraphicsDevice;

        protected override void Initialize()
        {
            _graphics.GraphicsDevice.Viewport = new Viewport(); // empty
        }

        protected override void Update(GameTime gameTime)
        {
        }

        protected override void Draw(GameTime gameTime)
        {
        }
    }
}