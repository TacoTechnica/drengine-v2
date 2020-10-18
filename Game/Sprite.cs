using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    /// <summary>
    ///     A sprite object.
    /// </summary>
    public class Sprite
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Pivot;
        public float Scale = 1;
        private GamePlus _game;

        public Sprite(GamePlus game, Texture2D texture)
        {
            this._game = game;
            this.Texture = texture;
        }

        public Sprite(GamePlus game, GamePath path, Vector2 Pivot, float Scale)
        {
            this._game = _game;
            this.Pivot = Pivot;
            this.Scale = Scale;
            Texture = Texture2D.FromFile(_game.GraphicsDevice, path);
        }

        public Sprite(GamePlus game, GamePath path) : this(game, path, Vector2.Zero, 1f) {}
    }
}
