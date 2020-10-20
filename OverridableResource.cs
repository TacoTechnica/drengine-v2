using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using DREngine.Game;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using Path = DREngine.Game.Path;

namespace DREngine
{

    public interface IOnDeserialized
    {
        void OnDeserialized(GraphicsDevice g);
    }

    class DefaultResourcePath : EnginePath {
        public DefaultResourcePath(string path) : base(System.IO.Path.Join("default_resources", path)) { }
    }

    public abstract class OverridableResource : IOnDeserialized
    {
        public string OverridePath = null;

        // TODO: Right now this is also serialized, but is there a smoother way?
        public string ContentPathDoNotModify = null;

        public OverridableResource(string contentPathDoNotModify)
        {
            // When we use the default constructor, we load right now.
            this.ContentPathDoNotModify = contentPathDoNotModify;
        }

        // Empty constructor (REQUIRED for serializable types)
        public OverridableResource()  { }

        /// <summary>
        /// Removes the override path, sticking with the default content path.
        /// </summary>
        public void RemoveOverride()
        {
            OverridePath = null;
        }

        public void OnDeserialized(GraphicsDevice g)
        {
            if (OverridePath != null)
            {
                Debug.Log($"{this} Loading Override: {OverridePath}");
                LoadOverride(g, OverridePath);
            }
            else
            {
                Debug.Log($"{this} Loading Default:  {ContentPathDoNotModify}");
                LoadDefault(g, ContentPathDoNotModify);
            }
        }

        protected abstract void LoadDefault(GraphicsDevice g, string path);
        protected abstract void LoadOverride(GraphicsDevice g, string path);
    }

    public class OverridableSpriteFont : OverridableResource, IOnDeserialized
    {
        [YamlIgnore]
        public SpriteFont Font { get; private set; } = null;

        public int Size = 16;

        public OverridableSpriteFont(string contentPathDoNotModify, int size) : base(contentPathDoNotModify)
        {
            this.Size = size;
        }

        public OverridableSpriteFont()
        {
        }

        protected override void LoadOverride(GraphicsDevice g, string path)
        {
            // TODO: Use project path.
            Load(g, new DefaultResourcePath(path));
        }

        protected override void LoadDefault(GraphicsDevice g, string path)
        {
            Load(g, new DefaultResourcePath(path));
        }

        private void Load(GraphicsDevice g, Path p)
        {
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(p),
                Size,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.Cyrillic
                }
            );

            Font = fontBakeResult.CreateSpriteFont(g);
        }
    }
}
