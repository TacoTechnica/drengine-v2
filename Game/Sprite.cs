using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    // TODO: Support holding animation data/multiple frames.
    /// <summary>
    ///     A sprite object.
    /// </summary>
    public class Sprite
    {
        public Texture2D Texture { get; private set; } = null;
        public Vector2 Pivot;
        public readonly float Scale;
        private GamePlus _game;

        private GamePath _path;

        public float Width => Texture.Width;
        public float Height => Texture.Height;

        public Sprite(GamePlus game, Texture2D texture)
        {
            this._game = game;
            this.Texture = texture;
        }

        public Sprite(GamePlus game, GamePath path, Vector2 Pivot, float Scale)
        {
            this._game = game;
            this.Pivot = Pivot;
            this.Scale = Scale;
            this._path = path;

            _game.WhenSafeToLoad.AddListener(LoadSprite);
        }

        public Sprite(GamePlus game, GamePath path) : this(game, path, Vector2.Zero, 0.01f) {}

        private void LoadSprite()
        {
            Texture = Texture2D.FromFile(_game.GraphicsDevice, _path);
            _game.WhenSafeToLoad.RemoveListener(LoadSprite);
        }
    }
}
