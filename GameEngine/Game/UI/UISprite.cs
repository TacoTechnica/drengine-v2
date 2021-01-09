using GameEngine.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            //screen.DrawRectOutline(targetRect, Color.Red);

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
                DrawSpriteTo(screen.SpriteBatch, Sprite, preservedTarget, Color);
            }
            else
            {
                // Stretch
                DrawSpriteTo(screen.SpriteBatch, Sprite, targetRect, Color);
            }

            screen.SpriteBatchEnd();
        }

        private void DrawSpriteTo(SpriteBatch batch, Sprite sprite, Rectangle destRect, Color col)
        {
            Margin m = sprite.ScaleMargin;
            // Game Maker draw 9 tile sprite vibes. Ah those were good times.
            int ox = destRect.Left,
                oy = destRect.Top;

            // Variable length sizes: Destination
            float lw = destRect.Width - m.Left - m.Right,
                  lh = destRect.Height - m.Top - m.Bottom;
            // Variable length sizes: Sprite
            float slw = sprite.Width - m.Left - m.Right,
                  slh = sprite.Height - m.Top - m.Bottom;

            // Upper left corner
            Section(ox, oy, m.Left, m.Top, 0, 0, m.Left, m.Top);
            // Top
            Section(ox + m.Left, oy, lw, m.Top, m.Left, 0, slw, m.Top );
            // Top right corner
            Section(ox + m.Left + lw, oy, m.Right, m.Top, m.Left + slw, 0, m.Right, m.Top );
            // Left
            Section(ox, oy + m.Top, m.Left, lh, 0, m.Top, m.Left, slh );
            // Right
            Section(ox + m.Left + lw, oy + m.Top, m.Right, lh, m.Left + slw, m.Top, m.Right, slh );
            // Bottom left corner
            Section(ox, oy + m.Top + lh, m.Left, m.Bottom, 0, m.Top + slh, m.Left, m.Bottom);
            // Bottom
            Section(ox + m.Left, oy + m.Top + lh, lw, m.Bottom, m.Left, m.Top + slh, slw, m.Bottom );
            // Bottom right corner
            Section(ox + m.Left + lw, oy + m.Top + lh, m.Right, m.Bottom, m.Left + slw, m.Top + slh, m.Right, m.Bottom );

            // Middle
            Section(ox + m.Left, oy + m.Top, lw, lh, m.Left, m.Top, slw, slh);

            //batch.Draw(Sprite.Texture, destRect, new Rectangle(0, 0, (int)sprite.Width, (int)sprite.Height), col);

            void Section(float x, float y, float width, float height, float sprX, float sprY, float sprW, float sprH)
            {
                Rectangle dest = new Rectangle((int)x, (int)y, (int)width, (int)height);
                Rectangle source = new Rectangle((int)sprX, (int)sprY, (int)sprW,(int) sprH);
                if (dest.Width <= 0 || dest.Height <= 0) return;
                batch.Draw(Sprite.Texture, dest, source, col);
            }
        }
    }
}