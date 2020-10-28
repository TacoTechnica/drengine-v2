using System.Text.Encodings.Web;
using DREngine.Game.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public class UIVectorText : UIComponent
    {
        private BasicEffect _effect;

        public VectorFont Font;

        public float Size = 16f;

        public string Text = "";

        public bool CullingEnabled = false;

        public UIVectorText(GamePlus game, VectorFont font, UIComponent parent = null) : base(game, parent)
        {
            Font = font;
        }

        protected override void Initialize()
        {
            _effect = new BasicEffect(_game.GraphicsDevice);
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {

            _effect.Projection = Matrix.Identity;
            _effect.View = Matrix.Identity;
            _effect.World = screen.CurrentWorld * screen.OpenGL2Pixel;

            _effect.World = Matrix.CreateTranslation(targetRect.Min.X, targetRect.Min.Y, 0) *  _effect.World;

            screen.GraphicsDevice.RasterizerState = CullingEnabled ? RasterizerState.CullClockwise : RasterizerState.CullNone;

            // TODO: Do same thing as text renderer and abstract out the rendering
            // Go through all passes, apply and draw the triangle for each pass.
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                Font.DrawString(_game.GraphicsDevice, pass, _effect, Size, Text, true);
            }
        }
    }
}
