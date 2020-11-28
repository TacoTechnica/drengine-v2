using GameEngine.Game;
using GameEngine.Game.UI;
using Microsoft.Xna.Framework;

namespace GameEngine.Test
{
    public class TestRichText : IGameRunner
    {
        private UIColoredRect _rect;
        private UIText _text;

        private const string TEST_TEXT =
            "\"Friends\" is a gritty documentary. The setting of this city in Hong Kong. Starring: Chandler, Soros, Phobos 1, Rachel, Monica. And some other fucking actors.";
        public void Initialize(GamePlus g)
        {
            TestGame game = (TestGame) g;
            _rect = (UIColoredRect) new UIColoredRect(game, Color.Black)
                .WithLayout(Layout.FullscreenLayout(64))
                .AddToRoot();
            _text = (UIText) new UIText(game, game.TestFont, TEST_TEXT, Color.White)
                .WithLayout(Layout.FullscreenLayout());
            _rect.AddChild(_text);
        }

        public void Update(float deltaTime)
        {

        }

        public void Draw()
        {

        }
    }
}
