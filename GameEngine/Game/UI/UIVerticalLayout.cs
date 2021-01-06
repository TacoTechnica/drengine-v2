namespace GameEngine.Game.UI
{
    public class UIVerticalLayout : UIComponent
    {
        public bool AutoResizeToChildren = true;
        public float ChildHeight;

        public bool ExpandWidth = true;
        public Padding Padding;

        public float Spacing;

        public UIVerticalLayout(GamePlus game, float childHeight, float spacing = 0, UIComponent parent = null) : base(
            game, parent)
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
                var targetHeight = Padding.Top + Padding.Bottom + ChildCount * (ChildHeight + Spacing);
                var dy = targetHeight - targetRect.Height;
                Layout.Margin.Bottom -= dy;
            }

            var i = 0;
            foreach (var child in Children)
            {
                var dy = Padding.Top + i * (ChildHeight + Spacing);
                var dx = Padding.Left;
                var childRect = child.LayoutRect;
                var childWidth = ExpandWidth ? targetRect.Width - (Padding.Left + Padding.Right) : childRect.Width;
                child.WithLayout(
                    Layout.CornerLayout(Layout.TopLeft, childWidth, ChildHeight)
                        .OffsetBy(dx, dy)
                );
                ++i;
            }
        }
    }
}