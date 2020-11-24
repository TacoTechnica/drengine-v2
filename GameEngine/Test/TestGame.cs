using GameEngine.Game;
using GameEngine.Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Test
{
    /// <summary>
    /// A test game class that we use for our tests.
    /// </summary>
    public class TestGame : GamePlus
    {
        public SpriteFont TestFont => DebugConsole.Font;

        private Color BACKGROUND => Color.SlateBlue;

        public TestGame() : base("Game Engine Test", "Content", true, new TestLayout()) {}

        #region Extra help with testing + simple background

        protected override void Update(GameTime gameTime)
        {
            if (RawInput.KeyPressed(Keys.F1))
            {
                Debug.LogDebug("Toggling Debug Collider Drawing");
                DebugDrawColliders = !DebugDrawColliders;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BACKGROUND);
            base.Draw(gameTime);
        }

        #endregion

        public static void Main(string[] args) {
            Debug.InitRootDirectory();
            using (var game = new TestGame()) {
                game.Run();
            }
        }
    }

}
