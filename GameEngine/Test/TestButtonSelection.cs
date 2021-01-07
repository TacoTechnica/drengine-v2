using GameEngine.Game;
using GameEngine.Game.Input;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Test
{
    public class TestButtonSelection : IGameRunner
    {
        private GamePlus _game;
        private ExampleMenuControls _menuControls;
        private MenuList _menu;
        public void Initialize(GamePlus game)
        {
            _game = game;

            _menuControls = new ExampleMenuControls(game);

            _menu = new MenuList(game, _menuControls.Select, _menuControls.SelectMouse, _menuControls.Down, _menuControls.Up);

            AddButton(0, "New Game");
            AddButton(1, "Load Game");
            AddButton(2, "Eat My Ass");
            AddButton(3, "Exit");
        }

        private void AddButton(int index, string text)
        {
            ExampleUiMenuButton b = new ExampleUiMenuButton(_game, text);
            b.WithLayout(Layout.CornerLayout(Layout.Bottom, 100, 60));
            b.OffsetBy(Vector2.UnitY * (-200 + 64 * index));
            b.AddToRoot();
            _menu.AddChild(b);
        }

        public void Update(float deltaTime)
        {

        }

        public void Draw()
        {

        }

        class ExampleMenuControls : Controls
        {
            public InputActionButton Select;
            public InputActionButton Up;
            public InputActionButton Down;
            public InputActionButton SelectMouse;
            public ExampleMenuControls(GamePlus game) : base(game)
            {
                Select = new InputActionButton(this, Keys.Space, Keys.Enter);
                SelectMouse = new InputActionButton(this, MouseButton.Left);
                Up = new InputActionButton(this, Keys.Up, Keys.W, Buttons.LeftThumbstickUp);
                Down = new InputActionButton(this, Keys.Down, Keys.S, Buttons.LeftThumbstickDown);
            }
        }


        class ExampleUiMenuButton : UIMenuButtonBase
        {

            private Font TextFont => ((TestGame)Game).TestFont;

            private Color _normalColor;
            private Color _selectedColor;
            private Color _pressColor;

            private Color _currentColor;

            private UIText _text;

            public ExampleUiMenuButton(GamePlus game, UIComponent parent, string text) : base(game, parent)
            {

                _normalColor = Color.DarkSlateGray;
                _selectedColor = Color.LightGoldenrodYellow;
                _pressColor = Color.IndianRed;

                _currentColor = _normalColor;

                _text = new UIText(game, TextFont, text, this)
                {
                    TextHAlign = UIText.TextHAlignMode.Center,
                    TextVAlign = UIText.TextVAlignMode.Middle
                };
                _text.Color = Color.White;
            }
            public ExampleUiMenuButton(GamePlus game, string text) : this(game, null, text) {}

            protected override void Draw(UIScreen screen, Rect targetRect)
            {
                screen.DrawRect(targetRect, _currentColor);
            }

            protected override void OnSelectVisual()
            {
                _currentColor = _selectedColor;
                _text.Color = Color.Black;
                //Debug.Log($"SELECT {_text}");
            }

            protected override void OnDeselectVisual()
            {
                _currentColor = _normalColor;
                _text.Color = Color.White;
                //Debug.Log($"deSELECT {_text}");
            }

            protected override void OnPressVisual()
            {
                // TODO: Animate out
                _currentColor = _pressColor;
            }

            protected override void OnDepressVisual()
            {
                // Do nothing
            }
        }

    }
}
