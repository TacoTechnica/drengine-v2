using Microsoft.Xna.Framework;
using YamlDotNet.Serialization.ObjectFactories;

namespace GameEngine.Game.UI
{
    public class UIVerticalLayout : UIComponent
    {

        public float Spacing;
        public Padding Padding;

        public bool ExpandWidth = true;
        public float ChildHeight;

        public bool AutoResizeToChildren = true;

        public UIVerticalLayout(GamePlus game, float childHeight, UIComponent parent = null) : base(game, parent)
        {
            ChildHeight = childHeight;
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            if (AutoResizeToChildren)
            {
                float targetHeight = Padding.Top + Padding.Bottom + ChildCount * (ChildHeight + Spacing);
                float dy = targetHeight - targetRect.Height;
                Layout.Margin.Bottom -= dy;
            }
            int i = 0;
            foreach (UIComponent child in Children)
            {
                float dy = Padding.Top + i * (ChildHeight + Spacing);
                float dx = Padding.Left;
                Rect childRect = child.LayoutRect;
                float childWidth = ExpandWidth ? targetRect.Width - (Padding.Left + Padding.Right) : childRect.Width;
                child.WithLayout(
                    Layout.CornerLayout(Layout.TopLeft, childWidth, ChildHeight)
                        .OffsetBy(dx, dy)
                );
                ++i;
            }
        }
    }
}
