
using Microsoft.Xna.Framework;

namespace DREngine.Game.UI
{
    /// <summary>
    /// A simple colored rectangle.
    /// </summary>
    public class UIColoredRect : UIComponent
    {
        public Color _color0, _color1, _color2, _color3;

        private bool _border = false;
        public UIColoredRect(GamePlus game, Color color0, Color color1, Color color2, Color color3, bool border = false, UIComponent parent = null) : base(game, parent)
        {
            _color0 = color0;
            _color1 = color1;
            _color2 = color2;
            _color3 = color3;
            _border = border;
        }

        public UIColoredRect(GamePlus game, Color color, bool border = false, UIComponent parent = null) : this(game, color, color, color, color, border, parent) {}

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            if (_border)
            {
                screen.DrawRectOutline(targetRect, _color0, _color1, _color2, _color3);
            }
            else
            {
                screen.DrawRect(targetRect, _color0, _color1, _color2, _color3);
            }
        }
    }
}
