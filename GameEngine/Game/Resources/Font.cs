using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpriteFontPlus;

namespace GameEngine.Game.Resources
{
    public class Font : IGameResource
    {
        public string Path { get; set; }
        public int Size;

        [JsonIgnore]
        public SpriteFont SpriteFont;

        public Font(GamePlus game, string path, int size)
        {
            Path = path;
            Size = size;
            Load(game);
        }

        // Deserialize constructor
        public Font()
        {
            Size = 12;
        }

        public void Load(GamePlus game)
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

            SpriteFont = fontBakeResult.CreateSpriteFont(game.GraphicsDevice);
        }

        public void Unload(GamePlus game)
        {
            SpriteFont = null;
        }
    }
}
