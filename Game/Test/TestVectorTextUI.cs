using System.IO;
using DREngine.Game.UI;
using Gdk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus;
using static DREngine.Game.Input.RawInput;
using Color = Microsoft.Xna.Framework.Color;

namespace DREngine.Game
{
    public class TestVectorTextUI : IGameRunner
    {
        private VectorFont vfont;

        private UIText _topLeftBitmapText;
        private UIVectorText _topLeftText;
        private UIVectorText _centerFocusText;

        public static float TEMP_TEST = 1f; // Scale
        public static float TEMP_TEST2 = 0f; // Offset

        public void Initialize(GamePlus game)
        {
            Path p = new EnginePath("default_resources/Fonts/HennyPenny/HennyPenny-Regular.ttf");
                //new EnginePath("default_resources/Fonts/SourceSansPro/SourceSansPro-RegularItalic.ttf");
                float SIZE = game.GraphicsDevice.Viewport.Height / 5f;

            SpriteFont sFont = LoadFont(game.GraphicsDevice, p, SIZE);
            vfont = new VectorFont(game, p);

            string testText = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA\nBBBBBBBBBBBBBBBBBBBBBBB\nHHHHHHHHHHHHHHHHHHHHHH";//"The quick brown fox jumped over the lazy dog.\nABCDEFGHIJKLMNOPQRSTUVWXYZ\nabcdefghijklmnopqrstuvwxyz\n.,!?@#$%^&*()-+-=\"';:\\|/<>\nHello there!";
            testText += "\nCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC\nDDDDDDDDDDDDDDDDDDDDDDDDDDDDD";
            _topLeftBitmapText = new UIText(game, sFont, testText);
            _topLeftBitmapText.WordWrap = false;
            _topLeftBitmapText.WithLayout(Layout.FullscreenLayout());
            _topLeftBitmapText.Color = Color.Black;
            _topLeftBitmapText.AddToRoot();

            _topLeftText = new UIVectorText(game, vfont);
            _topLeftText.Text = testText;
            _topLeftText.Layout.Pivot = Vector2.Zero;
            _topLeftText.Size = SIZE;
            _topLeftText.AddToRoot();


            _centerFocusText = (UIVectorText)new UIVectorText(game, vfont).WithLayout(Layout.CenteredLayout(20, 20)).AddToRoot();
            _centerFocusText.Text = "ll_ll";
            _centerFocusText.Size = 128;

        }

        public void Update(float deltaTime)
        {
            // We good
            Vector3 r = Math.ToEuler(_topLeftText.LocalTransform.Rotation);
            if (KeyPressing(Keys.Right))
            {
                TEMP_TEST += 0.001f;
                Debug.Log($"VAL: {TEMP_TEST}, {TEMP_TEST2}");
            }
            if (KeyPressing(Keys.Left))
            {
                TEMP_TEST -= 0.001f;
                Debug.Log($"VAL: {TEMP_TEST}, {TEMP_TEST2}");
            }
            if (KeyPressing(Keys.Up))
            {
                TEMP_TEST2 += 0.0002f;
                Debug.Log($"VAL: {TEMP_TEST}, {TEMP_TEST2}");
            }
            if (KeyPressing(Keys.Down))
            {
                TEMP_TEST2 -= 0.0002f;
                Debug.Log($"VAL: {TEMP_TEST}, {TEMP_TEST2}");
            }
            if (KeyPressing(Keys.D))
            {
                r.Z += 120 * deltaTime;
            }
            if (KeyPressing(Keys.A))
            {
                r.Z -= 120 * deltaTime;
            }
            _topLeftText.LocalTransform.Rotation = Math.FromEuler(r);
        }

        public void Draw()
        {
            // We good
        }

        private static SpriteFont LoadFont(GraphicsDevice g, Path path, float Size)
        {
            var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(path),
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

            return fontBakeResult.CreateSpriteFont(g);
        }
    }
}
