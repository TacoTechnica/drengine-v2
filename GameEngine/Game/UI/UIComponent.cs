
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameEngine.Game.UI
{
    public abstract class UIComponent: UIComponentBase
    {
        protected UIComponentBase _parent;

        private ObjectContainerNode<UIComponent> _addedNode;

        /// <summary>
        /// General component constructor with a parent
        /// </summary>
        public UIComponent(GamePlus game, UIComponent parent=null) : base(game)
        {
            _parent = parent;

            if (parent != null)
            {
                _parent.AddChild(this);
            }
        }

        internal void ReceiveParent(UIComponentBase parent, ObjectContainerNode<UIComponent> node)
        {
            _parent?.RemoveEnqueueChild(_addedNode);
            _parent = parent;
            _addedNode = node;
        }

        public override void DestroyImmediate()
        {
            _parent?.RemoveEnqueueChild(_addedNode);
            base.DestroyImmediate();
        }

        public UIComponent CopyLayoutFrom(UIComponentBase toCopy)
        {
            _copyLayoutFrom = toCopy;
            return this;
        }

        public UIComponent WithLayout(Layout layout)
        {
            Layout = layout;
            return this;
        }

        public UIComponent WithChild(params UIComponent[] children)
        {
            foreach (UIComponent c in children)
            {
                AddChild(c);
            }

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
            if (_parent != null)
            {
                throw new InvalidOperationException("Tried to set our UI parent to root when we already have a parent!");
            }

            _game.UiScreen.AddRootChild(this, forceBase);

            return this;
        }

        public ObjectContainerNode<UIComponent> GetParentListNode()
        {
            return _addedNode;
        }

        protected override Rect GetParentRect()
        {
            return _parent.LayoutRect;
        }
    }

    public class UIContainer : UIComponent

    {
        public UIContainer(GamePlus game, UIComponent parent = null) : base(game, parent) {}

        protected override void Draw(UIScreen screen, Rect targetRect)
        {
            // Do nothing
        }
    }
}
