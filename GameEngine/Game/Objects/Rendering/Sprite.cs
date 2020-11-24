using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game
{
    // TODO: Support holding animation data/multiple frames.
    /// <summary>
    ///     A sprite object.
    /// </summary>
    public class Sprite
    {
        private Texture2D _texture;

        public Texture2D Texture
        {
            get
            {
                if (_texture == null)
                {
                    throw new InvalidOperationException("Tried accessing sprite texture before it was made.\n" +
                                                        "MAKE SURE SPRITE ACCESS IS DONE IN Start(), and NOT in a constructor!!");
                }

                return _texture;
            }
            private set => _texture = value;
        }

        public Vector2 Pivot;
        public readonly float Scale;
        private GamePlus _game;

        private Path _path;

        public float Width => Texture.Width;
        public float Height => Texture.Height;

        public bool Loaded { get; private set; }

        public Sprite(GamePlus game, Texture2D texture)
        {
            this._game = game;
            this.Texture = texture;
            Loaded = true;
        }

        public Sprite(GamePlus game, Path path, Vector2 Pivot, float Scale = 0.01f)
        {
            this._game = game;
            this.Pivot = Pivot;
            this.Scale = Scale;
            this._path = path;

            Loaded = false;
            _game.LoadWhenSafe(LoadSprite);
        }

        public Sprite(GamePlus game, Path path) : this(game, path, Vector2.Zero) {}

        private void LoadSprite()
        {
            Texture = Texture2D.FromFile(_game.GraphicsDevice, _path);
            Loaded = true;
        }
    }
}
