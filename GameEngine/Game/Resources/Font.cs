using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpriteFontPlus;

namespace GameEngine.Game.Resources
{
    public class Font : IGameResource
    {
        private GamePlus _game;

        [ExtraData] public int Size;

        [JsonIgnore] public SpriteFont SpriteFont;

        public Font(GamePlus game, Path path, int size)
        {
            _game = game;
            Path = path;
            Size = size;
            Load(game.ResourceLoaderData);
        }

        // Deserialize constructor
        public Font()
        {
            Size = 12;
        }

        public Path Path { get; set; }

        public void Load(ResourceLoaderData data)
        {
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(Path),
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

            SpriteFont = fontBakeResult.CreateSpriteFont(data.GraphicsDevice);

            ExtraResourceHelper.LoadExtraData(this, Path);
        }

        public void Save(Path path)
        {
            Path = path;
            ExtraResourceHelper.SaveExtraData(this, path);
        }

        public void Unload()
        {
            SpriteFont = null;
        }

        ~Font()
        {
            Unload();
        }
    }
}