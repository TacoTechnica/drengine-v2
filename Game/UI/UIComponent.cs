
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DREngine.Game.UI
{
    public abstract class UiComponent: UIComponentBase
    {
        protected UIComponentBase _parent;

        private ObjectContainerNode<UiComponent> _addedNode;

        /// <summary>
        /// General component constructor with a parent
        /// </summary>
        public UiComponent(GamePlus game, UiComponent parent=null) : base(game)
        {
            _parent = parent;

            if (parent != null)
            {
                _parent.AddChild(this);
            }
        }

        internal void ReceiveParent(UIComponentBase parent, ObjectContainerNode<UiComponent> node)
        {
            _parent?.RemoveEnqueueChild(_addedNode);
            _parent = parent;
            _addedNode = node;
        }

        public UiComponent WithLayout(Layout layout)
        {
            Layout = layout;
            return this;
        }

        public UiComponent WithChild(params UiComponent[] children)
        {
            foreach (UiComponent c in children)
            {
                AddChild(c);
            }

            return this;
        }


        public UiComponent OffsetBy(float x, float y)
        {
            Layout.OffsetBy(x, y);
            return this;
        }

        public UiComponent OffsetBy(Vector2 pos)
        {
            return OffsetBy(pos.X, pos.Y);
        }

        public UiComponent WithPivot(float x, float y)
        {
            Layout.Pivot = new Vector2(x, y);
            return this;
        }

        public UiComponent AddToRoot()
        {
            if (_parent != null)
            {
                throw new InvalidOperationException("Tried to set our UI parent to root when we already have a parent!");
            }

            _game.UiScreen.AddChild(this);

            return this;
        }




    }
}
