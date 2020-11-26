using Microsoft.Xna.Framework;
using YamlDotNet.Serialization.ObjectFactories;

namespace GameEngine.Game.UI
{
    public class UIHorizontalLayout : UIComponent
    {

        public float Spacing;
        public Padding Padding;

        public bool ExpandHeight = true;
        public float ChildWidth;

        public bool AutoResizeToChildren = true;

        public UIHorizontalLayout(GamePlus game, float childWidth, float spacing = 0, UIComponent parent = null) : base(game, parent)
        {
            ChildWidth = childWidth;
            Spacing = 0;
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            if (AutoResizeToChildren)
            {
                float targetWidth = Padding.Top + Padding.Bottom + ChildCount * (ChildWidth + Spacing);
                float dx = targetWidth - targetRect.Width;
                Layout.Margin.Right -= dx;
            }
            int i = 0;
            foreach (UIComponent child in Children)
            {
                float dx = Padding.Left + i * (ChildWidth + Spacing);
                float dy = Padding.Top;
                Rect childRect = child.LayoutRect;
                float childHeight = ExpandHeight ? targetRect.Height - (Padding.Top + Padding.Bottom) : childRect.Height;
                child.WithLayout(
                    Layout.CornerLayout(Layout.TopLeft, ChildWidth, childHeight)
                        .OffsetBy(dx, dy)
                );
                ++i;
            }
        }
    }
}
