using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DREngine
{
    public class DRGame : Game
    {
        #region Util variables

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        #endregion

        private static DRGame _instance = null;
        public static DRGame Instance
        {
            get
            {
                if (_instance == null) Debug.LogError("No Engine Instance, everything will blow up");
                return _instance;
            }
        }

        public DRGame()
        {
            _instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.Window.Title = "Game Engine Test";
            this._graphics.SynchronizeWithVerticalRetrace = true;

        }

        protected override void Initialize()
        {
            // Init
            base.Initialize();

            Debug.LogDebug("Game Initialize()");
        }

        protected override void LoadContent()
        {
            // Init load
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw
            base.Draw(gameTime);
        }
    }
}
