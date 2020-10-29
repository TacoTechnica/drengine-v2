using DREngine.Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game.UI
{
    public abstract class UIBaseComponent
    {
        protected GamePlus _game;

        public Transform3D LocalTransform = new Transform3D();

        public Layout Layout = new Layout();

        private readonly ObjectContainer<UIComponent> _children = new ObjectContainer<UIComponent>();

        private bool _isMasked = false;

        private int _maskIndex = 0;
        //private RenderTarget2D _maskedRenderTarget = null;

        public UIBaseComponent(GamePlus game)
        {
            _game = game;
        }

        public void AddChild(UIComponent child)
        {
            if (this is UIMask mask)
            {
                child._isMasked = true;
                child._maskIndex = mask.MaskIndex;
            }
            child.ReceiveParent(this, _children.Add(child));
        }

        public void RemoveEnqueueChild(ObjectContainerNode<UIComponent> toRemove)
        {
            // TODO: Where is this ^ used? Shouldn't you pass the child object instead?
            _children.RemoveEnqueue(toRemove);
        }

        public void RunOnDestroy()
        {
            // Delete all children too.
            _children.LoopThroughAll((child) =>
            {
                child.RunOnDestroy();
            });
            // Cleanup, may as well empty the list.
            _children.RemoveAllQueuedImmediate();
        }

        public void DoDraw(UIScreen screen, Matrix worldMat, Rect targetRect)
        {
            screen.CurrentWorld = worldMat;

            if (_isMasked)
            {


                // Allow masking
                screen.GraphicsDevice.DepthStencilState = new DepthStencilState
                {
                    StencilEnable = true,
                    StencilFunction = CompareFunction.LessEqual,
                    StencilPass = StencilOperation.Keep,
                    ReferenceStencil = _maskIndex,
                    DepthBufferEnable = false,
                };
            }

            Draw(screen, targetRect);

            bool childSelected = false;


            _children.LoopThroughAllAndDeleteQueued(
                child =>
                {
                    Matrix childMat = child.LocalTransform.Local;
                    // Transform around pivot
                    Rect target = child.Layout.GetTargetRect(targetRect);
                    Vector2 pivotPos = target.Min + target.Size * child.Layout.Pivot;
                    screen.CurrentWorld = Matrix.CreateTranslation(-pivotPos.X, -pivotPos.Y, 0) * childMat * Matrix.CreateTranslation(pivotPos.X, pivotPos.Y, 0) * worldMat;
                    child.DoDraw(screen, screen.CurrentWorld, target);

                    // If any child is selected after the corresponding draw call, mark that.
                    if (!childSelected && screen.NeedToUpdateSelectables && child is ICursorSelectable selectable)
                    {
                        if (selectable.__ChildWasSelected || selectable.CursorSelected) childSelected = true;
                    }
                }
            );

            // if our object asks for it, do selection checking.
            if (screen.NeedToUpdateSelectables && this is ICursorSelectable selectable)
            {
                selectable.__ChildWasSelected = childSelected;

                Vector2 cursorPos = _game.CurrentCursor.Position;
                bool isCursorMoving = _game.CurrentCursor.MovedLastFrame;
                bool prevSelected = selectable.CursorSelected;
                bool selected;
                // If a child was selected, we might want to ignore this selection.
                if (selectable.ChildrenSelectFirst && childSelected)
                {
                    selected = false;
                }
                else
                {
                    selected = targetRect.Contains(cursorPos);
                }

                if (isCursorMoving)
                {
                    selectable.CursorSelected = selected;
                }

                if (selectable.CursorSelected && !prevSelected)
                {
                    // If we are part of a parent menu, inform the parent that we've been selected.
                    if (selectable is IMenuItem menuItem)
                    {
                        if (menuItem.ParentMenu != null && menuItem.ParentMenu.UseMouse)
                        {
                            menuItem.ParentMenu.SetSelected(menuItem);
                        }
                    }

                    selectable.OnCursorSelect();
                }
                else if (!selectable.CursorSelected && prevSelected)
                {
                    selectable.OnCursorDeselect();
                }

            }
        }

        protected abstract void Draw(UIScreen screen, Rect targetRect);
    }
}
