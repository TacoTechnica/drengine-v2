using GameEngine.Game;
using GameEngine.Game.Input;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Test
{
    public class TestScroll : IGameRunner
    {
        private UISlider _slider;
        private UIText _text;

        private TestGame _game;

        private UIScrollView _scrollView;
        private UIComponent _contents;

        private Font _textFont => _game.TestFont;

        private const string contentText = "BOO!! Sorry did I scare you?! WASSUP GURL😉😉😊 ITS ALMOST COCKTOBER 😈🌚🍂🍃🍁 AND IF YOU👈🏽 ARE GETTING THIS👇🏽😘 IT MEANS UR A HALLOWEEN 👻🎃 HOE😏😩👅💦 every year in Cocktober the jack o slut🎃 comes to life🙀😻🙌🏽👏👏🙌🏽 coming to harvest 🍁🍂🍃 his hoes for THOT-O-WEEN😏😏💥💥🎈🎂🎉 send this to 10 other Halloween Hoes or else you a TRICK🎃👻👻 🎃 IF YOU GET 4 BACK UR A THOT-O-WEEN TREAT😋 IF YOU GET 6 BACK UR A SLUTTY WITCH BITCH👄😍✨🔮 BUT IF YOU GET 10 BACK UR THE SPOOKIEST SLUT ON THE BLOCK😜💦⚰🎉🎉💯🎃 If you don’t send this to 1️⃣0️⃣other thots💁😩👄 you will get NO DICK 👋 this COCKTOBER🎃";

        public void Initialize(GamePlus game)
        {
            _game = (TestGame) game;
            _slider = (UISlider) new UISlider(game)
                .WithLayout(Layout.SideStretchLayout(Layout.Right, 16f, 8f).OffsetBy(-8, 0))
                .AddToRoot();
            _text = (UIText) new UIText(game, _textFont, "(empty)")
                .WithHAlign(UIText.TextHAlignMode.Center)
                .WithoutWordWrap()
                .WithLayout(Layout.CenteredLayout(0,0))
                .AddToRoot();

            _contents = new UIColoredRect(game, Color.Firebrick)
                .WithChild(
                    new UIText(game, _textFont, contentText, Color.White)
                ).WithLayout(Layout.CornerLayout(Layout.TopLeft, 100, 600));
            //_scrollView = new UIScrollView(game, _contents, new UIColoredRect(game, Color.Green), _slider)
            UIComponent viewBox = new UIColoredRect(game, Color.Moccasin, true)
                .WithLayout(Layout.CustomLayout(0, 0, 0.5f, 0.5f, 10, 10, 10, 10))
                .AddToRoot();
            _scrollView = (UIScrollView) new UIScrollViewMasked(game, _contents, _slider);
            viewBox.AddChild(_scrollView);
        }

        public void Update(float deltaTime)
        {
            int percent = (int) (100 * _slider.SlidePercent);

            _text.Text = $"P: {percent}%";
            if (RawInput.KeyPressed(Keys.Up))
            {
                _slider.SlidePercent -= 0.1f;
                Debug.Log($"UP: {_slider.SlidePercent}");
            }
            if (RawInput.KeyPressed(Keys.Down))
            {
                _slider.SlidePercent += 0.1f;
                Debug.Log($"DOWN: {_slider.SlidePercent}");
            }
        }

        public void Draw() { }

    }
}
