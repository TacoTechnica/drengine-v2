using System;
using System.Text.Json.Serialization;
using GameEngine.Game.Resources;
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
        public float Scale;

        [JsonIgnore]
        public Path Path { get; set; }

        [JsonIgnore]
        public float Width => Texture.Width;
        [JsonIgnore]
        public float Height => Texture.Height;

        [JsonIgnore]
        public bool Loaded { get; private set; }

        private GamePlus _game;

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
                Load(_game.ResourceLoaderData);
            });
        }

        public Sprite(GamePlus game, Path path) : this(game, path, Vector2.Zero) {}

        // Deserialize Constructor
        public Sprite() {}

        ~Sprite()
        {
            _texture?.Dispose();
        }

        public virtual void Load(ResourceLoaderData data)
        {
            //Debug.Log($"LOADING SPRITE: {Path}");
            Texture = Texture2D.FromFile(data.GraphicsDevice, Path);
            Loaded = true;
            // TODO: Load extra data
        }

        public virtual void Save(Path path)
        {
            Path = path;
            // TODO: Save extra data
        }

        public virtual void Unload()
        {
            Texture.Dispose();
            Loaded = false;
        }
    }
}
