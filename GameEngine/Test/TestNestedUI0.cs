using GameEngine.Game;
using GameEngine.Game.Input;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using GameEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Test
{
    public class TestNestedUI0 : IGameTester
    {
        private GamePlus _game;

        private UIComponent _root;
        private UIComponent _rotater;
        private UIComponent _rotater2;

        private Font TextFont => ((TestGame)_game).TestFont;
        public void Initialize(GamePlus game)
        {
            _game = game;

            // Right side Bar
            _root = new UIBoxPanel(_game, Color.Red, Color.Pink)
                .AddToRoot()
                .WithLayout(Layout.FullscreenLayout(100, 10, 10, 10))
                .WithChild(
                    // Bottom right Corner-er
                    new UIBoxPanel(_game, Color.Green, Color.Lime)
                        .WithLayout(Layout.CornerLayout(Layout.BottomRight, 60, 60))
                        .OffsetBy(-4, -4),
                    new UIBoxPanel(_game, Color.Yellow, Color.LightYellow)
                        .WithLayout(Layout.SideStretchLayout(Layout.Top, 100, 4))
                        .WithChild(new UIText(_game, TextFont, "Hello there!")
                        {
                            TextHAlign = UIText.TextHAlignMode.Right,
                            TextVAlign = UIText.TextVAlignMode.Bottom
                        })
                        .WithChild(new UIBoxPanel(_game, Color.Brown, Color.Wheat)
                            .WithLayout(Layout.FullscreenLayout(200, 10, 200, 10 ))
                        )
                );
            // Left side Bar
            _rotater2 = new UIBoxPanel(_game, Color.Blue, Color.Aqua)
                .AddToRoot()
                .WithLayout(Layout.CornerLayout(Layout.TopLeft, 50, 100))
                .OffsetBy(0, 0)
                .WithPivot(0f, 0f);

            _rotater = _root;
        }

        public void Update(float deltaTime)
        {
            Vector3 e = Math.ToEuler(_rotater.LocalTransform.Rotation);
            // Rotate and scale for fun
            if (RawInput.KeyPressing(Keys.Right))
            {
                e.Z += 90f * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.Left))
            {
                e.Z -= 90f * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.Up))
            {
                _rotater.LocalTransform.Scale += Vector3.UnitX * deltaTime;
                _rotater2.LocalTransform.Scale += Vector3.UnitY * deltaTime;
            }
            if (RawInput.KeyPressing(Keys.Down))
            {
                _rotater.LocalTransform.Scale -= Vector3.UnitX * deltaTime;
                _rotater2.LocalTransform.Scale -= Vector3.UnitY * deltaTime;
            }


            _rotater.LocalTransform.Rotation = Math.FromEuler(e);
            e *= -1;
            _rotater2.LocalTransform.Rotation = Math.FromEuler(e);
        }

        public void Draw()
        {
            // Nothing, UI is doing the work in the background!
        }

        class UIBoxPanel : UIComponent, ICursorSelectable
        {
            private Color _deselectColor;
            private Color _selectColor;

            private Color _currentColor;
            public UIBoxPanel(GamePlus game, UIComponent parent, Color deselectColor, Color selectColor) : base(game, parent)
            {
                _deselectColor = deselectColor;
                _selectColor = selectColor;
                _currentColor = _deselectColor;
            }
            public UIBoxPanel(GamePlus game, Color c, Color selectColor) : this(game, null, c, selectColor){}

            protected override void Draw(UIScreen screen, Rect targetRect)
            {
                // Fill the rect with our box
                screen.DrawRect(targetRect.X, targetRect.Y, targetRect.Width, targetRect.Height, _currentColor);
            }

            public bool CursorSelected { get; set; } = false;
            public bool __ChildWasSelected { get; set; } = false;
            public bool ChildrenSelectFirst { get; set; } = true;

            public void OnCursorSelect()
            {
                _currentColor = _selectColor;
            }

            public void OnCursorDeselect()
            {
                _currentColor = _deselectColor;
            }
        }
    }


}
