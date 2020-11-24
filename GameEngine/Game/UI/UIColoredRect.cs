
using Microsoft.Xna.Framework;

namespace GameEngine.Game.UI
{
    /// <summary>
    /// A simple colored rectangle.
    /// </summary>
    public class UIColoredRect : UIComponent
    {
        public Color Color0, Color1, Color2, Color3;

        private bool _border = false;
        public UIColoredRect(GamePlus game, Color color0, Color color1, Color color2, Color color3, bool border = false, UIComponent parent = null) : base(game, parent)
        {
            Color0 = color0;
            Color1 = color1;
            Color2 = color2;
            Color3 = color3;
            _border = border;
        }

        public UIColoredRect(GamePlus game, Color color, bool border = false, UIComponent parent = null) : this(game, color, color, color, color, border, parent) {}

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            if (_border)
            {
                screen.DrawRectOutline(targetRect, Color0, Color1, Color2, Color3);
            }
            else
            {
                screen.DrawRect(targetRect, Color0, Color1, Color2, Color3);
            }
        }

        public void SetColor(Color color)
        {
            Color0 = color;
            Color1 = color;
            Color2 = color;
            Color3 = color;
        }
    }
}
