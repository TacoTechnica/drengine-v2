using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game
{
    public class TestSpriteRendering : IGameStarter
    {
        private GamePlus _game;

        private SpriteRenderer _sr;
        private Camera3D _cam;

        private SpriteBatch _spriteBatch; // TESTING ONLY

        public void Initialize(GamePlus game)
        {
            _game = game;
            Sprite sprite = new Sprite(_game, new EnginePath("projects/test_project/Sprites/Obama.png"));
            _cam = new Camera3D(_game, Vector3.Backward * 10, Math.FromEuler(0, 0, 0));
            _sr = new SpriteRenderer(_game, sprite, Vector3.Zero, Quaternion.Identity);
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public void Update(float deltaTime)
        {
            // Rotate sprite while we hold R
            if (Input.IsPressing(Keys.F))
            {
                _sr.Rotation = Math.FromEuler(Math.ToEuler(_sr.Rotation) + Vector3.Up * 90 * deltaTime);
            }
            // Rotate camera
            if (Input.IsPressing(Keys.R))
            {
                Vector3 e = Math.ToEuler(_cam.Rotation);
                e.Y += 90f * deltaTime;
                _cam.Rotation = Math.FromEuler(e);
            }

            if (Input.IsPressing(Keys.Right))
            {
                _sr.Scale.X += 1f * deltaTime;
            }
            if (Input.IsPressing(Keys.Left))
            {
                _sr.Scale.X -= 1f * deltaTime;
            }
        }

        public void Draw()
        {
            // You can uncomment this to draw the sprite in a 2D fashion, normally.
            /*
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _spriteBatch.Draw(_sr.Sprite.Texture, Vector2.Zero, Color.White);
            _spriteBatch.End();
            */
        }
    }
}
