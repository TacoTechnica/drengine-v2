using System;
using GameEngine.Game.Objects;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.UI
{
    public abstract class UIComponent : UIComponentBase
    {
        private ObjectContainerNode<UIComponent> _addedNode;
        protected UIComponentBase Parent;

        /// <summary>
        ///     General component constructor with a parent
        /// </summary>
        public UIComponent(GamePlus game, UIComponent parent = null) : base(game)
        {
            Parent = parent;

            if (parent != null) Parent.AddChild(this);
        }

        internal void ReceiveParent(UIComponentBase parent, ObjectContainerNode<UIComponent> node)
        {
            Parent?.RemoveEnqueueChild(_addedNode);
            Parent = parent;
            _addedNode = node;
        }

        public override void DestroyImmediate()
        {
            Parent?.RemoveEnqueueChild(_addedNode);
            base.DestroyImmediate();
        }

        public new UIComponent CopyLayoutFrom(UIComponentBase toCopy)
        {
            base.CopyLayoutFrom = toCopy;
            return this;
        }

        public UIComponent WithLayout(Layout layout)
        {
            Layout = layout;
            return this;
        }

        public UIComponent WithChild(params UIComponent[] children)
        {
            foreach (var c in children) AddChild(c);

            return this;
        }


        public UIComponent OffsetBy(float x, float y)
        {
            Layout.OffsetBy(x, y);
            return this;
        }

        public UIComponent OffsetBy(Vector2 pos)
        {
            return OffsetBy(pos.X, pos.Y);
        }

        public UIComponent WithPivot(float x, float y)
        {
            Layout.Pivot = new Vector2(x, y);
            return this;
        }

        public UIComponent AddToRoot(bool forceBase = false)
        {
            if (Parent != null)
                throw new InvalidOperationException(
                    "Tried to set our UI parent to root when we already have a parent!");

            Game.UiScreen.AddRootChild(this, forceBase);

            return this;
        }

        public ObjectContainerNode<UIComponent> GetParentListNode()
        {
            return _addedNode;
        }

        protected override Rect GetParentRect()
        {
            return Parent.LayoutRect;
        }
    }

    public class UIContainer : UIComponent

    {
        public UIContainer(GamePlus game, UIComponent parent = null) : base(game, parent)
        {
        }

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            // Do nothing
        }
    }
}