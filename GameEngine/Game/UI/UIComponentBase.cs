using System.Collections.Generic;
using GameEngine.Game.Input;
using GameEngine.Game.Objects;
using GameEngine.Game.Tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game.UI
{
    public abstract class UIComponentBase
    {
        private readonly ObjectContainer<UIComponent> _children = new ObjectContainer<UIComponent>();
        //private RenderTarget2D _maskedRenderTarget = null;

        protected UIComponentBase CopyLayoutFrom = null;
        protected GamePlus Game;

        private bool _isMasked;

        private Rect _layoutRect;

        private UIMask _mask;

        public bool Active = true;

        public Layout Layout = new Layout();

        public Transform3D LocalTransform = new Transform3D();

        public UIComponentBase(GamePlus game)
        {
            Game = game;
            Tweener = new TweenerUI(game, this);
        }

        public Rect LayoutRect
        {
            get
            {
                // TODO: Comment out true to avoid recalculation.
                if (true || _layoutRect == null) _layoutRect = Layout.GetTargetRect(GetParentRect());
                return _layoutRect;
            }
            set => _layoutRect = value;
        }

        public IEnumerable<UIComponent> Children => _children;
        public int ChildCount => _children.Count;

        public TweenerUI Tweener { get; }

        public void AddChild(UIComponent child)
        {
            // If we already have our child.
            if (_children.Contains(child.GetParentListNode())) return;
            if (this is UIMask mask)
            {
                child._isMasked = true;
                child._mask = mask;
            }
            else if (_isMasked)
            {
                child._isMasked = true;
                child._mask = _mask;
            }

            child.ReceiveParent(this, _children.Add(child));
        }

        internal void RemoveEnqueueChild(ObjectContainerNode<UIComponent> toRemove)
        {
            // TODO: Where is this ^ used? Shouldn't you pass the child object instead?
            _children.RemoveEnqueue(toRemove);
        }

        public virtual void DestroyImmediate()
        {
            // Delete all children too.
            _children.LoopThroughAll(child =>
            {
                _children.RemoveEnqueue(child.GetParentListNode());
                child.DestroyImmediate();
            });
            // Cleanup, may as well empty the list.
            _children.RemoveAllQueuedImmediate();
        }

        public void DoDraw(UIScreen screen, Matrix worldMat, Rect targetRect)
        {
            screen.OnUIDraw(Active);

            if (!Active) return;

            if (screen.NeedToUpdateControl) Tweener.RunUpdate();

            if (CopyLayoutFrom != null) Layout = new Layout(CopyLayoutFrom.Layout);

            screen.CurrentWorld = worldMat;

            var prevStencil = screen.GraphicsDevice.DepthStencilState;
            if (_isMasked)
                // Allow masking
                screen.GraphicsDevice.DepthStencilState = new DepthStencilState
                {
                    StencilEnable = true,
                    StencilFunction = CompareFunction.Equal,
                    StencilPass = StencilOperation.Keep,
                    ReferenceStencil = _mask.MaskIndex,
                    DepthBufferEnable = false
                };

            LayoutRect = targetRect;
            Draw(screen, targetRect);

            var childSelected = false;

            _children.LoopThroughAllAndDeleteQueued(
                child =>
                {
                    var childMat = child.LocalTransform.Local;
                    // Transform around pivot
                    var target = child.Layout.GetTargetRect(targetRect);
                    var pivotPos = target.Min + target.Size * child.Layout.Pivot;
                    screen.CurrentWorld = Matrix.CreateTranslation(-pivotPos.X, -pivotPos.Y, 0) * childMat *
                                          Matrix.CreateTranslation(pivotPos.X, pivotPos.Y, 0) * worldMat;
                    child.DoDraw(screen, screen.CurrentWorld, target);

                    // If any child is selected after the corresponding draw call, mark that.
                    if (!childSelected && screen.NeedToUpdateControl && child is ICursorSelectable selectableChild)
                        if (selectableChild.__ChildWasSelected || selectableChild.CursorSelected)
                            childSelected = true;
                }
            );

            if (_isMasked) screen.GraphicsDevice.DepthStencilState = prevStencil;

            // if our object asks for it, do selection checking.
            if (screen.NeedToUpdateControl && this is ICursorSelectable selectable)
            {
                selectable.__ChildWasSelected = childSelected;

                var cursorPos = Game.CurrentCursor.Position;
                var isCursorMoving = Game.CurrentCursor.MovedLastFrame;
                var prevSelected = selectable.CursorSelected;
                bool selected;
                // If a child was selected, we might want to ignore this selection.
                if (selectable.ChildrenSelectFirst && childSelected)
                    selected = false;
                else
                    selected = targetRect.Contains(cursorPos);

                // Selection can be obfuscated by the mask.
                if (_isMasked) selected = selected && _mask.LayoutRect.Contains(cursorPos);

                if (isCursorMoving) selectable.CursorSelected = selected;

                var newlySelected = selectable.CursorSelected && !prevSelected;

                if (newlySelected)
                {
                    // If we are part of a parent menu, inform the parent that we've been selected.
                    if (selectable is IMenuItem menuItem)
                    {
                        if (menuItem.ParentMenu != null)
                        {
                            if (menuItem.ParentMenu.UseMouse) menuItem.ParentMenu.SetSelected(menuItem);
                        }
                        else
                        {
                            // No parent. Use default stuff.
                            if (!menuItem.MenuSelected)
                            {
                                menuItem.MenuSelected = true;
                                menuItem.OnMenuSelect();
                            }
                        }
                    }

                    selectable.OnCursorSelect();
                }
                else if (!selectable.CursorSelected && prevSelected)
                {
                    // Deselected
                    selectable.OnCursorDeselect();
                    // If we are a parent-less menu item, deselect manually by mouse.
                    if (selectable is IMenuItem menuItem)
                        if (menuItem.ParentMenu == null)
                            if (menuItem.MenuSelected)
                            {
                                menuItem.MenuSelected = false;
                                menuItem.OnMenuDeselect();
                            }
                }

                // Now handle pressing if we are a menu item and we have no parent.
                if (RawInput.MousePressed(MouseButton.Left) && selectable is IMenuItem menuItemm)
                    if (menuItemm.ParentMenu == null)
                    {
                        if (menuItemm.MenuSelected)
                            menuItemm.OnMenuPress(true);
                        else
                            menuItemm.OnMenuDepress(true);
                    }
            }
        }

        protected virtual Rect GetParentRect()
        {
            return Game.UiScreen.LayoutRect;
        }

        protected abstract void Draw(UIScreen screen, Rect targetRect);


        public override string ToString()
        {
            return GetType().Name + (Active ? "" : " (not active) ");
        }
    }
}