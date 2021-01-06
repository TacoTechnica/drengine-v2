using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game.UI
{
    public abstract class UIMask : UIComponent
    {
        private static int MaskCounter = 1;

        private readonly DepthStencilState _maskStencil;

        public UIMask(GamePlus game, UIComponent parent = null) : base(game, parent)
        {
            MaskIndex = MaskCounter++;
            _maskStencil = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = MaskIndex,
                DepthBufferEnable = false
            };
        }

        //public RenderTarget2D RenderTarget { get; protected set; }= null;

        public int MaskIndex { get; }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            var prev = screen.GraphicsDevice.DepthStencilState;
            screen.GraphicsDevice.DepthStencilState = _maskStencil;
            DrawMask(screen, targetRect);
            screen.GraphicsDevice.DepthStencilState = prev;
        }

        protected abstract void DrawMask(UIScreen screen, Rect targetRect);
    }

    public class UIMaskRect : UIMask
    {
        public Color Color;

        public UIMaskRect(GamePlus game, UIComponent parent = null) : base(game, parent)
        {
            Color = Color.Black;
            Color.A = 0;

            /*
            if (separateTarget)
            {
                RenderTarget = new RenderTarget2D(game.GraphicsDevice, game.GraphicsDevice.Viewport.Width,
                    game.GraphicsDevice.Viewport.Height);
            }
            */
        }

        public UIMaskRect(GamePlus game, Color backgroundColor, UIComponent parent = null) : base(game, parent)
        {
            Color = backgroundColor;
        }

        protected override void DrawMask(UIScreen screen, Rect targetRect)
        {
            screen.DrawRect(targetRect, Color);
        }
    }
}