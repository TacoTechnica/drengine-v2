using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game.UI
{
    public abstract class UIMask : UIComponent
    {
        private static int MaskCounter = 1;

        private DepthStencilState _maskStencil;

        //public RenderTarget2D RenderTarget { get; protected set; }= null;

        public int MaskIndex { get; private set; }

        public UIMask(GamePlus game, UIComponent parent = null) : base(game, parent)
        {
            MaskIndex = MaskCounter++;
            _maskStencil = new DepthStencilState {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = MaskIndex,
                DepthBufferEnable = false,
            };

        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            DepthStencilState prev = screen.GraphicsDevice.DepthStencilState;
            screen.GraphicsDevice.DepthStencilState = _maskStencil;
            DrawMask(screen, targetRect);
            screen.GraphicsDevice.DepthStencilState = prev;
        }

        protected abstract void DrawMask(UIScreen screen, Rect targetRect);
    }

    public class UIMaskRect : UIMask
    {
        private bool _transparent;
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

        protected override void DrawMask(UIScreen screen, Rect targetRect)
        {
            screen.DrawRect(targetRect, Color);
        }
    }
}
