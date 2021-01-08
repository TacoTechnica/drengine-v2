using GameEngine.Game;
using GameEngine.Game.Input;
using GameEngine.Game.Objects.Rendering;
using GameEngine.Game.Resources;
using GameEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#pragma warning disable 169

namespace GameEngine.Test
{
    public class TestSpriteRendering : IGameTester
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
            //_spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public void Update(float deltaTime)
        {

            // Rotate sprite while we hold R
            if (RawInput.KeyPressed(Keys.F))
            {
                Debug.Log($"SCREEN POS: {_cam.WorldCoordToScreenCoord(_sr.Transform.Position)}, MOUSE: {RawInput.GetMousePosition()}");
            }

            Vector3 r = Math.ToEuler(_sr.Transform.Rotation);
            if (RawInput.KeyPressing(Keys.Right))
            {
                r.Y += 120 * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.Left))
            {
                r.Y -= 120 * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.Up))
            {
                r.X += 120 * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.Down))
            {
                r.X -= 120 * deltaTime;
            }
            _sr.Transform.Rotation = Math.FromEuler(r);

            Vector3 e = Math.ToEuler(_cam.Rotation);
            // Rotate camera
            if (RawInput.KeyPressing(Keys.A))
            {
                e.Y += 90f * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.D))
            {
                e.Y -= 90f * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.W))
            {
                e.X += 90f * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.S))
            {
                e.X -= 90f * deltaTime;
            }
            _cam.Rotation = Math.FromEuler(e);

            if (RawInput.KeyPressing(Keys.Right))
            {
                _sr.Transform.Scale += 1f * deltaTime * Vector3.UnitX;
            }
            if (RawInput.KeyPressing(Keys.Left))
            {
                _sr.Transform.Scale -= 1f * deltaTime * Vector3.UnitX;
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
