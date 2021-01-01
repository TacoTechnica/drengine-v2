
namespace GameEngine.Game.UI
{
    public class UIVerticalLayout : UIComponent
    {

        public float Spacing;
        public Padding Padding;

        public bool ExpandWidth = true;
        public float ChildHeight;

        public bool AutoResizeToChildren = true;

        public UIVerticalLayout(GamePlus game, float childHeight, float spacing = 0,UIComponent parent = null) : base(game, parent)
        {
            ChildHeight = childHeight;
            Spacing = spacing;
        }

        public UIVerticalLayout PadLeft(float left)
        {
            Padding.Left = left;
            return this;
        }
        public UIVerticalLayout PadRight(float right)
        {
            Padding.Right = right;
            return this;
        }
        public UIVerticalLayout PadTop(float top)
        {
            Padding.Top = top;
            return this;
        }
        public UIVerticalLayout PadBottom(float bottom)
        {
            Padding.Bottom = bottom;
            return this;
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
