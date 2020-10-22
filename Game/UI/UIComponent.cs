
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DREngine.Game.UI
{
    public abstract class UIComponent: UIBaseComponent
    {
        protected UIBaseComponent _parent;

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

        internal void ReceiveParent(UIBaseComponent parent, ObjectContainerNode<UIComponent> node)
        {
            if (_parent != null)
            {
                _parent.RemoveEnqueueChild(_addedNode);
            }
            _parent = parent;
            _addedNode = node;
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

        public UIComponent WithPivot(float x, float y)
        {
            Layout.Pivot = new Vector2(x, y);
            return this;
        }

        public UIComponent AddToRoot()
        {
            if (_parent != null)
            {
                throw new InvalidOperationException("Tried to set our UI parent to root when we already have a parent!");
            }

            _game.UIScreen.AddChild(this);

            return this;
        }




    }
}
