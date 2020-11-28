using System;
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
            "\"Friends\" is a <b>gritty</b> documentary. The <#FF0000>setting</color> of <#FF00FF>this</color> <i>city</i> in Hong Kong.\nStarring:\nChandler, Soros, Phobos 1, Rachel, Monica. And <#00FF00>some other</color> fucking actors.";
        public void Initialize(GamePlus g)
        {
            TestGame game = (TestGame) g;
            _rect = (UIColoredRect) new UIColoredRect(game, Color.Black)
                .WithLayout(Layout.FullscreenLayout(64))
                .AddToRoot();
            _text = (UIText) new UIText(game, game.TestFont, TEST_TEXT, Color.White)
                .WithRichText()
                .WithPivot(0.5f, 0.5f)
                .WithLayout(Layout.FullscreenLayout());
            _rect.AddChild(_text);
        }

        public void Update(float deltaTime)
        {

            Vector3 euler = Math.ToEuler(_text.LocalTransform.Rotation);
            euler.Z += 0.2f;
            _text.LocalTransform.Rotation = Math.FromEuler(euler);

            /*
            float counter = 0;
            for (int i = 0; i < 9000000; i++)
            {
                counter += 1f / i;
            }
            Debug.Log($"SLOWDOWN COUNTER: {counter}");
            */
        }

        public void Draw()
        {

        }
    }
}
