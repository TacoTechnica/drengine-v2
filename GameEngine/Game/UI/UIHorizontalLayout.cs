namespace GameEngine.Game.UI
{
    public class UIHorizontalLayout : UIComponent
    {
        public bool AutoResizeToChildren = true;
        public float ChildWidth;

        public bool ExpandHeight = true;
        public Padding Padding;

        public float Spacing;

        public UIHorizontalLayout(GamePlus game, float childWidth, float spacing = 0, UIComponent parent = null) : base(
            game, parent)
        {
            ChildWidth = childWidth;
            Spacing = 0;
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            if (AutoResizeToChildren)
            {
                var targetWidth = Padding.Top + Padding.Bottom + ChildCount * (ChildWidth + Spacing);
                var dx = targetWidth - targetRect.Width;
                Layout.Margin.Right -= dx;
            }

            var i = 0;
            foreach (var child in Children)
            {
                var dx = Padding.Left + i * (ChildWidth + Spacing);
                var dy = Padding.Top;
                var childRect = child.LayoutRect;
                var childHeight = ExpandHeight ? targetRect.Height - (Padding.Top + Padding.Bottom) : childRect.Height;
                child.WithLayout(
                    Layout.CornerLayout(Layout.TopLeft, ChildWidth, childHeight)
                        .OffsetBy(dx, dy)
                );
                ++i;
            }
        }
    }
}