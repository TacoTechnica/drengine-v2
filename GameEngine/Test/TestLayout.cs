using GameEngine.Game;
using GameEngine.Game.Input;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using GameEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Test
{
    /// <summary>
    ///     Use the numpad to switch between layout modes.
    ///     Use the arrow keys to adjust spacing
    ///     Hold shift while pressing numpad keys to adjust padding instead.
    /// </summary>
    public class TestLayout : IGameTester
    {
        private string[] tileNames = new[]
        {
            "I come from",
            "the forest where",
            "words are",
            "dangerous.",
            "What is",
            "my name?"
        };

        private UIGridLayout _gridLayout;
        private UIVerticalLayout _verticalLayout;
        private UIHorizontalLayout _horizontalLayout;

        public void Initialize(GamePlus game)
        {
            _gridLayout = (UIGridLayout) new UIGridLayout(game)
                .WithLayout(Layout.FullscreenLayout(0))
                .AddToRoot();
            _verticalLayout = (UIVerticalLayout) new UIVerticalLayout(game, 48)
                .WithLayout(Layout.FullscreenLayout())
                .AddToRoot();
            _horizontalLayout = (UIHorizontalLayout) new UIHorizontalLayout(game, 64)
                .WithLayout(Layout.FullscreenLayout())
                .AddToRoot();

            _verticalLayout.Spacing = 16;
            _horizontalLayout.Spacing = 16;
            _verticalLayout.Active = false;
            _horizontalLayout.Active = false;

            // Add children
            foreach (string name in tileNames)
            {
                _gridLayout.AddChild(
                    new UITestItem(game, ((TestGame)game).TestFont, name)
                );
                _verticalLayout.AddChild(
                    new UITestItem(game, ((TestGame)game).TestFont, name)
                );
                _horizontalLayout.AddChild(
                    new UITestItem(game, ((TestGame)game).TestFont, name)
                );
            }
        }

        public void Update(float deltaTime)
        {
            if (RawInput.KeyPressed(Keys.NumPad0))
            {
                Debug.Log("Switched to Grid Layout");
                _gridLayout.Active = true;
                _verticalLayout.Active = false;
                _horizontalLayout.Active = false;
            } else if (RawInput.KeyPressed(Keys.NumPad1))
            {
                Debug.Log("Switched to Vertical Layout");
                _gridLayout.Active = false;
                _verticalLayout.Active = true;
                _horizontalLayout.Active = false;
            }  else if (RawInput.KeyPressed(Keys.NumPad2))
            {
                Debug.Log("Switched to Horizontal Layout");
                _gridLayout.Active = false;
                _verticalLayout.Active = false;
                _horizontalLayout.Active = true;
            }


            float dsx = (RawInput.KeyPressing(Keys.Right) ? 1 : 0) - (RawInput.KeyPressing(Keys.Left) ? 1 : 0);
            float dsy = (RawInput.KeyPressing(Keys.Down) ? 1 : 0) - (RawInput.KeyPressing(Keys.Up) ? 1 : 0);

            Vector2 delta = 32f * deltaTime * new Vector2(dsx, dsy);

            if (_gridLayout.Active)
            {
                if (RawInput.KeyPressing(Keys.LeftShift))
                {
                    _gridLayout.Padding.Left += delta.X;
                    _gridLayout.Padding.Right += delta.X;
                    _gridLayout.Padding.Top += delta.Y;
                    _gridLayout.Padding.Bottom += delta.Y;
                }
                else
                {
                    _gridLayout.Spacing += delta;
                }
            }

            if (_verticalLayout.Active)
            {
                if (RawInput.KeyPressing(Keys.LeftShift))
                {
                    _verticalLayout.ChildHeight += delta.Y;
                }
                else
                {
                    _verticalLayout.Spacing += delta.Y;
                }

                _verticalLayout.Padding.Left += delta.X;
                _verticalLayout.Padding.Right += delta.X;
                _verticalLayout.Padding.Top += delta.X;
                _verticalLayout.Padding.Bottom += delta.X;
            }
            if (_horizontalLayout.Active)
            {
                if (RawInput.KeyPressing(Keys.LeftShift))
                {
                    _horizontalLayout.ChildWidth += delta.X;
                }
                else
                {
                    _horizontalLayout.Spacing += delta.X;
                }

                _horizontalLayout.Padding.Left += delta.Y;
                _horizontalLayout.Padding.Right += delta.Y;
                _horizontalLayout.Padding.Top += delta.Y;
                _horizontalLayout.Padding.Bottom += delta.Y;
            }

        }

        public void Draw() { }

        class UITestItem : UIComponent
        {
            public UITestItem(GamePlus game, Font font, string text, UIComponent parent = null) : base(game, parent)
            {
                new UIColoredRect(game, Random.Hue(0.5f, 0.7f), false, this)
                    .WithChild(
                        new UIText(game, font, text, Color.White)
                            //.WithoutWordWrap()
                            .WithHAlign(UIText.TextHAlignMode.Center)
                            .WithVAlign(UIText.TextVAlignMode.Middle)
                            .WithLayout(Layout.FullscreenLayout())
                    );
            }

            protected override void Draw(UIScreen screen, Rect targetRect) { }
        }
    }
}
