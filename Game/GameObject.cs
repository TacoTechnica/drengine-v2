using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{
    public abstract class GameObject : IGameObject
    {
        protected GamePlus _game;

        private bool _gottaStart = true;
        private LinkedListNode<GameObject> _gameAddedNode = null;

        private ObjectContainer<GameObject> _childObjects = new ObjectContainer<GameObject>();

        // To catch circular parenting which would break the game badly.
        private HashSet<GameObject> _parents = new HashSet<GameObject>();

        private bool _alive = false;

        public GameObject(GamePlus game)
        {
            _game = game;
            _gameAddedNode = game.SceneManager.GameObjects.Add(this);
            _gottaStart = true;
            _alive = true;
            Awake();
        }

        ~GameObject()
        {
            // TODO: Destroy immediate?
        }

        #region Public Control

        public void AddChild(GameObject obj)
        {
            AssertAlive();
            if (obj._parents.Contains(this))
            {
                throw new InvalidOperationException($"Attempted to add child to parent more than once! Child: {obj}, Parent: {this}");
            }

            if (_parents.Contains(obj))
            {
                throw new InvalidOperationException($"Circular Parenting Detected when trying to add child! Child: {obj}, Parent: {this}");
            }
            obj._parents.Add(this);
            _childObjects.Add(obj);
        }
        internal void RunUpdate(float dt)
        {
            AssertAlive();
            // Start before first tick
            EnsureStarted();
            Update(dt);
        }

        internal virtual void RunOnDestroy()
        {
            AssertAlive();
            _gameAddedNode = null;

            // Delete all children too.
            _childObjects.LoopThroughAll((child) =>
            {
                child.Destroy();
            });
            // Cleanup, may as well empty the list.
            _childObjects.RemoveAllQueuedImmediate();

            // Kinda redundant but good to clean up.
            _parents.Clear();

            OnDestroy();

            _alive = false;
        }

        public void Destroy()
        {
            AssertAlive();
            if (!_alive) return;
            _game.SceneManager.GameObjects.RemoveEnqueue(_gameAddedNode);
        }

        public void DestroyImmediate()
        {
            AssertAlive();
            if (!_alive) return;
            _game.SceneManager.GameObjects.RemoveImmediate(_gameAddedNode, (self) => {self.RunOnDestroy();});
        }

        #endregion

        #region Utils

        protected void EnsureStarted()
        {
            AssertAlive();
            if (_gottaStart)
            {
                Start();
                _gottaStart = false;
            }
        }

        private void AssertAlive()
        {
            if (!_alive)
            {
                throw new InvalidOperationException($"Object is destroyed but you still tried accessing it! Object: {this}");
            }
        }

        #endregion

        #region Interface
        /// <summary>
        ///
        /// </summary>
        public virtual void Awake()
        {

        }

        public  virtual void Start()
        {

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dt"> delta time in seconds </param>
        public  virtual void Update(float dt)
        {

        }

        public virtual void OnDestroy()
        {

        }
        #endregion

        // When we've been destroyed, we can be compared to null.
        public static bool operator ==(GameObject obj, object other)
        {
            if (obj._alive)
            {
                return obj.Equals(other);
            }
            else
            {
                return (Object.Equals(other, null));
            }
        }

        public static bool operator !=(GameObject obj, object other)
        {
            return !(obj == other);
        }
    }
}
