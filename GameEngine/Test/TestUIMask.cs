using GameEngine.Game.Input;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Game.Test
{

    public class TestUIMask : IGameRunner
    {
        private UIComponent _mover;

        private TestGame _game;

        private SpriteFont _textFont => _game.TestFont;

        public void Initialize(GamePlus game)
        {
            _game = (TestGame) game;

            // Make sure stuff renders behind the mask
            new UIColoredRect(game, Color.OrangeRed)
                .WithLayout(Layout.SideStretchLayout(Layout.Top, 59, 0))
                .WithChild(new UIText(game, _textFont, "This part is not cropped and it will mind its own damn business."))
                .AddToRoot();

            // LEFT MASK
            UIComponent leftMask = new UIMaskRect(game)
                .WithLayout(Layout.ScaledLayout(0.1f, 0.1f, 0.4f, 0.9f))
                .AddToRoot()
                .WithChild(new UIColoredRect(game, Color.Chartreuse, true).WithLayout(Layout.FullscreenLayout(1, 1, 1, 1)));
            leftMask.AddChild(
                new UIText(game, _textFont, "This should be cropped partially as this keeps going yadayadayadayada sads adsa dsad sada sdas dsa dasd ")
                    .WithoutWordWrap()
                    .OffsetBy(-100, 0)
            );

            // mover (within LEFT MASK)
            _mover = new UIColoredRect(game, Color.Red, Color.Aqua, Color.DarkOliveGreen, Color.Coral)
                .WithLayout(Layout.CenteredLayout(50, 50))
                .WithChild(new UIText(game, _textFont, "Hello I should crop too"));
            leftMask.AddChild(_mover);


            // RIGHT MASK
            UIComponent rightMask = new UIMaskRect(game)
                .WithLayout(Layout.ScaledLayout(0.5f, 0, 0.9f, 0.9f))
                .AddToRoot()
                .WithChild(new UIColoredRect(game, Color.Gold, true).WithLayout(Layout.FullscreenLayout(1, 1, 1, 1)));
            rightMask.AddChild(
                new UIText(game, _textFont, "AAAAAAAAAAAAAAAAAAAA This should ALSO be cropped TWICE WILL REPEAT: This should ALSO be cropped")
                    .WithoutWordWrap()
                    .OffsetBy(-250, 150)
                );

        }

        public void Update(float deltaTime)
        {
            Vector2 move =
                Vector2.UnitX * ((RawInput.KeyPressing(Keys.Right) ? 1 : 0) - (RawInput.KeyPressing(Keys.Left) ? 1 : 0))
                + Vector2.UnitY * ((RawInput.KeyPressing(Keys.Down) ? 1 : 0) - (RawInput.KeyPressing(Keys.Up) ? 1 : 0));

            move *= 200 * deltaTime;

            //Debug.Log($"{move}, {_mover.Layout.Margin.Min}");

            _mover.Layout.OffsetBy(move.X, move.Y);

            /*
            _mover.LocalTransform.Position =
                _mover.LocalTransform.Position + Vector3.Right * move.X + Vector3.Up * move.Y;
                */
        }

        public void Draw()
        {
        }
    }
}
