using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.UI
{
    public class UISprite : UIComponent
    {
        public Color Color = Color.White;

        public bool PreserveAspect = false;
        public Sprite Sprite;

        public UISprite(GamePlus game, Sprite sprite, UIComponent parent = null) : base(game, parent)
        {
            Sprite = sprite;
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            if (Sprite == null) return;

            screen.SpriteBatchBegin();
            if (PreserveAspect)
            {
                var texAspect = Sprite.Width / Sprite.Height;
                var outerAspect = targetRect.Width / targetRect.Height;

                var targetWidth = targetRect.Width;
                var targetHeight = targetRect.Height;

                if (texAspect > outerAspect)
                    // Constrained by width
                    targetHeight = targetWidth / texAspect;
                else
                    // Constrained by height
                    targetWidth = targetHeight * texAspect;
                var preservedTarget = new Rectangle(
                    (int) (targetRect.X + targetRect.Width / 2f - targetWidth / 2f),
                    (int) (targetRect.Y + targetRect.Height / 2f - targetHeight / 2f),
                    (int) targetWidth,
                    (int) targetHeight
                );
                screen.SpriteBatch.Draw(Sprite.Texture, preservedTarget, Color);
            }
            else
            {
                // Stretch
                screen.SpriteBatch.Draw(Sprite.Texture, targetRect, Color);
            }

            screen.SpriteBatchEnd();
        }
    }
}