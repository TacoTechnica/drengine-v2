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

        public UIBaseComponent(GamePlus game)
        {
            _game = game;
        }

        public void AddChild(UIComponent child)
        {
            child.ReceiveParent(this, _children.Add(child));
        }

        public void RemoveEnqueueChild(ObjectContainerNode<UIComponent> toRemove)
        {
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
            Draw(screen, targetRect);

            // TODO: Get transformed rect and send it

            _children.LoopThroughAllAndDeleteQueued(
                child =>
                {
                    Matrix childMat = child.LocalTransform.Local;
                    // Transform around pivot
                    Rect target = child.Layout.GetTargetRect(targetRect);
                    Vector2 pivotPos = target.Min + target.Size * child.Layout.Pivot;
                    screen.CurrentWorld = Matrix.CreateTranslation(-pivotPos.X, -pivotPos.Y, 0) * childMat * Matrix.CreateTranslation(pivotPos.X, pivotPos.Y, 0) * worldMat;
                    child.DoDraw(screen, screen.CurrentWorld, target);
                }
            );
        }

        protected abstract void Draw(UIScreen screen, Rect targetRect);
    }
}
