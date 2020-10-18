using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public abstract class GameObjectRender : GameObject, IGameObject
    {

        private LinkedListNode<GameObjectRender> _renderAddedNode = null;

        protected GameObjectRender(GamePlus game) : base(game)
        {
            _renderAddedNode = game.SceneManager.GameRenderObjects.Add(this);
        }

        #region Internal Control

        internal void DoDraw(GraphicsDevice g)
        {
            EnsureStarted();
            Draw(g);
        }
        internal void DoPreDraw(GraphicsDevice g)
        {
            EnsureStarted();
            PreDraw(g);
        }
        internal void DoPostDraw(GraphicsDevice g)
        {
            EnsureStarted();
            PostDraw(g);
        }
        #endregion

        #region Util

        /// <summary>
        ///     When we are deleted, also delete ourselves off of the
        ///     render thing.
        /// </summary>
        internal override void RunOnDestroy()
        {
            _renderAddedNode = _game.SceneManager.GameRenderObjects.RemoveImmediate(_renderAddedNode);
            base.RunOnDestroy();
        }

        #endregion

        #region Object Functions

        /// <summary>
        /// Draws. Abstract cause we don't want to
        /// mistakenly make tons of game-object-renderers without
        /// ensuring that its rendering is actually taken
        /// advantage of.
        /// </summary>
        /// <param name="g"></param>
        public abstract void Draw(GraphicsDevice g);

        public virtual void PreDraw(GraphicsDevice g)
        {

        }
        public virtual void PostDraw(GraphicsDevice g)
        {

        }

        #endregion

    }
}
