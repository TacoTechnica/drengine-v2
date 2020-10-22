﻿using DREngine.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game
{
    public class TestNestedUI0 : IGameRunner
    {
        private GamePlus _game;

        private UIComponent _root;
        private UIComponent _rotater;
        private UIComponent _rotater2;

        public void Initialize(GamePlus game)
        {
            _game = game;

            // Right side Bar
            _root = new UIBoxPanel(_game, Color.Red)
                .AddToRoot()
                .WithLayout(Layout.FullscreenLayout(100, 10, 10, 10))
                .WithChild(
                    // Bottom right Corner-er
                    new UIBoxPanel(_game, Color.Green)
                        .WithLayout(Layout.CornerLayout(Layout.BottomRight, 60, 60))
                        .OffsetBy(-4, -4),
                    new UIBoxPanel(_game, Color.Yellow)
                        .WithLayout(Layout.SideLayout(Layout.Top, 30, 4))
                );
            // Left side Bar
            _rotater2 = new UIBoxPanel(_game, Color.Blue)
                .AddToRoot()
                .WithLayout(Layout.CornerLayout(Layout.TopLeft, 50, 100))
                .OffsetBy(40, 40)
                .WithPivot(0.9f, 0.9f);
            _rotater = _root;
        }

        public void Update(float deltaTime)
        {
            Vector3 e = Math.ToEuler(_rotater.LocalTransform.Rotation);
            // Rotate and scale for fun
            if (Input.KeyPressing(Keys.Right))
            {
                e.Z += 90f * deltaTime;
            }
            if (Input.KeyPressing(Keys.Left))
            {
                e.Z -= 90f * deltaTime;
            }
            if (Input.KeyPressing(Keys.Up))
            {
                _rotater.LocalTransform.Scale += Vector3.UnitX * deltaTime;
                _rotater2.LocalTransform.Scale += Vector3.UnitY * deltaTime;
            }
            if (Input.KeyPressing(Keys.Down))
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
    }

    class UIBoxPanel : UIComponent
    {
        private Color _color;
        public UIBoxPanel(GamePlus game, UIComponent parent, Color color) : base(game, parent)
        {
            _color = color;
        }
        public UIBoxPanel(GamePlus game, Color c) : this(game, null, c){}

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            // Fill the rect with our box
            screen.DrawRect(targetRect.X, targetRect.Y, targetRect.Width, targetRect.Height, _color);
        }

    }
}