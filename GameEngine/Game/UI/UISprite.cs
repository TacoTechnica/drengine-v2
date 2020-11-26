using Microsoft.Xna.Framework;

namespace GameEngine.Game.UI
{
    public class UISprite : UIComponent
    {
        public Sprite Sprite = null;
        public Color Color = Color.White;

        public bool PreserveAspect = false;

        public UISprite(GamePlus game, Sprite sprite, UIComponent parent = null) : base(game, parent)
        {
            Sprite = sprite;
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            screen.SpriteBatchBegin();
            if (PreserveAspect)
            {
                float texAspect = Sprite.Width / Sprite.Height;
                float outerAspect = targetRect.Width / targetRect.Height;

                float targetWidth = targetRect.Width;
                float targetHeight = targetRect.Height;

                if (texAspect > outerAspect)
                {
                    // Constrained by width
                    targetHeight = targetWidth / texAspect;
                }
                else
                {
                    // Constrained by height
                    targetWidth = targetHeight * texAspect;
                }
                Rectangle preservedTarget = new Rectangle(
                    (int) (targetRect.X + (targetRect.Width / 2f) - targetWidth / 2f),
                    (int) (targetRect.Y + (targetRect.Height / 2f) - targetHeight / 2f),
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
