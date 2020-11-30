using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SpriteFontPlus;

namespace GameEngine.Game.Resources
{
    public class Font : IGameResource
    {
        public Path Path { get; set; }
        public int Size;

        [JsonIgnore]
        public SpriteFont SpriteFont;

        private GamePlus _game;

        public Font(GamePlus game, Path path, int size)
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

        ~Font()
        {
            Debug.Log("OOF");
            Unload(_game);
        }

        public void Load(GamePlus game)
        {
            _game = game;

            game.Disposed += GameOnDisposed;

            // TODO: Load extra data
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

        private void GameOnDisposed(object? sender, EventArgs e)
        {
            if (SpriteFont != null)
            {
                Unload(_game);
            }
        }

        public void Save(Path path)
        {
            Path = path;
            // TODO: Save extra data
        }

        public void Unload(GamePlus game)
        {
            SpriteFont = null;
        }
    }
}
