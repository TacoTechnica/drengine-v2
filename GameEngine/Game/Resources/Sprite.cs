using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game
{
    /// <summary>
    ///     A sprite object.
    /// </summary>
    ///
    public class Sprite : IGameResource
    {
        private Texture2D _texture = null;

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

        public Path Path { get; set; }

        public float Width => Texture.Width;
        public float Height => Texture.Height;

        public bool Loaded { get; private set; }

        /*
        public Sprite(GamePlus game, Texture2D texture)
        {
            this._game = game;
            this.Texture = texture;
            Loaded = true;
        }
        */

        public Sprite(GamePlus game, Path path, Vector2 Pivot, float Scale = 0.01f)
        {
            this._game = game;
            this.Pivot = Pivot;
            this.Scale = Scale;
            this.Path = path;

            Loaded = false;
            _game.LoadWhenSafe(() =>
            {
                Load(_game);
            });
        }

        public Sprite(GamePlus game, Path path) : this(game, path, Vector2.Zero) {}

        // Deserialize Constructor
        public Sprite() {}

        ~Sprite()
        {
            _texture?.Dispose();
        }

        public void Load(GamePlus game)
        {
            //Debug.Log($"LOADING SPRITE: {Path}");
            _game = game;
            Texture = Texture2D.FromFile(game.GraphicsDevice, Path);
            Loaded = true;
            // TODO: Load extra data
        }

        public void Save(Path path)
        {
            Path = path;
            // TODO: Save extra data
        }

        public void Unload(GamePlus game)
        {
            Texture.Dispose();
            Loaded = false;
        }
    }
}
