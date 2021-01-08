using GameEngine.Game;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;
// ReSharper disable RedundantArgumentDefaultValue

namespace GameEngine.Test
{
    public class TestTextInput : IGameTester
    {

        private TestGame _game;

        private Font TextFont => _game.TestFont;

        public void Initialize(GamePlus game)
        {
            _game = (TestGame) game;

            float textHeight = 32,
                textSidePad = 10;

            UITextInput inputLeft = new UITextInput(game, TextFont, Color.White);
            UITextInput inputRight = new UITextInput(game,TextFont, Color.Coral);

            // Left box
            new UIColoredRect(game, Color.DarkSlateBlue, false)
                .WithLayout(Layout.CustomLayout(0, 0.5f, 0.5f, 0.5f, textSidePad, -0.5f*textHeight, textSidePad, -0.5f*textHeight))
                .WithChild(
                    inputLeft
                ).AddToRoot();
            // Right box
            new UIColoredRect(game, Color.DarkMagenta, false)
                .WithLayout(Layout.CustomLayout(0.5f, 0.5f, 1f, 0.5f, textSidePad, -0.5f*textHeight, textSidePad, -0.5f*textHeight))
                .WithChild(
                    inputRight
                ).AddToRoot();

            // Output example
            UIText textLeft = (UIText) new UIText(game, TextFont, "(empty)")
                .WithoutWordWrap()
                .WithLayout(Layout.CornerLayout(Layout.BottomLeft, 1000, 64))
                .AddToRoot();
            UIText textRight = (UIText) new UIText(game, TextFont, "(empty)")
                .WithoutWordWrap()
                .WithLayout(Layout.CornerLayout(Layout.BottomRight, 200, 64))
                .AddToRoot();

            // Example submission
            inputLeft.Submitted += s => textLeft.Text = $"SUBMITTED: {s}";
            inputRight.Submitted += s => textRight.Text = $"SUBMITTED: {s}";

        }

        public void Update(float deltaTime)
        {
            // Nothing for now
        }

        public void Draw()
        {
            // Nothing
        }
    }
}
