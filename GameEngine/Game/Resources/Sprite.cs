using System;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game.Resources
{
    /// <summary>
    ///     A sprite object.
    /// </summary>
    ///
    public class Sprite : IGameResource
    {
        private Texture2D _texture;

        [ExtraData]
        public Vector2 Pivot;

        [ExtraData]
        public float Scale = 1;

        [ExtraData]
        public Margin ScaleMargin = new Margin();

        /*
        public Sprite(GamePlus game, Texture2D texture)
        {
            this._game = game;
            this.Texture = texture;
            Loaded = true;
        }
        */

        public Sprite(GamePlus game, Path path, Vector2 pivot, float scale = 0.01f)
        {
            Pivot = pivot;
            Scale = scale;
            Path = path;

            Loaded = false;
            game.ResourceLoaderData.LoadWhenSafe(() =>
            {
                Load(game.ResourceLoaderData);
            });
        }

        public Sprite(GamePlus game, Path path) : this(game, path, Vector2.Zero) {}

        // Deserialize Constructor
        public Sprite() {}

        public Texture2D Texture
        {
            get
            {
                if (_texture == null)
                    throw new InvalidOperationException("Tried accessing sprite texture before it was made.\n" +
                                                        "MAKE SURE SPRITE ACCESS IS DONE IN Start(), and NOT in a constructor!!");

                return _texture;
            }
            private set => _texture = value;
        }

        public float Width => Texture.Width;
        public float Height => Texture.Height;

        public bool Loaded { get; private set; }

        public Path Path { get; set; }

        public virtual void Load(ResourceLoaderData data)
        {
            //Debug.Log($"LOADING SPRITE: {Path.GetShortName()}");
            Texture = Texture2D.FromFile(data.GraphicsDevice, Path);
            ExtraResourceHelper.LoadExtraData(this, Path);
            Loaded = true;
        }

        public virtual void Save(Path path)
        {
            Path = path;
            ExtraResourceHelper.SaveExtraData(this, Path);
        }

        public virtual void Unload()
        {
            Texture.Dispose();
            Loaded = false;
        }

        ~Sprite()
        {
            _texture?.Dispose();
        }
    }
}
