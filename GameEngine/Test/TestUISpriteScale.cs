using GameEngine.Game;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
// ReSharper disable UnusedType.Global

namespace GameEngine.Test
{
    public class TestUISpriteScale : IGameTester
    {
        private const string SPRITE_PATH = "projects/test_project/Sprites/Soda.png";

        public void Initialize(GamePlus game)
        {
            Sprite sprite = new Sprite(game, new EnginePath(SPRITE_PATH));

            sprite.ScaleMargin = new Margin(50, 50, 50, 50);

             new UISprite(game, sprite)
                .WithLayout(Layout.FullscreenLayout(20))
                .AddToRoot();
        }

        public void Update(float deltaTime)
        {
        }

        public void Draw()
        {
        }
    }
}