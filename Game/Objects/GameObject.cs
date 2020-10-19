using System;
using System.Collections;
using System.Collections.Generic;

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

        private List<Coroutine> _routines = new List<Coroutine>();
        private List<ICollider> _colliders = new List<ICollider>();

        public GameObject(GamePlus game)
        {
            _game = game;
            _gameAddedNode = game.SceneManager.GameObjects.Add(this);
            _gottaStart = true;
            _alive = true;
            Awake();
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
            // Add all of our parents and ourselves to the child.
            obj._parents.UnionWith(_parents);
            obj._parents.Add(this);
            _childObjects.Add(obj);
        }

        public void AddCollider(ICollider c)
        {
            AssertAlive();
            _colliders.Add(c);
            // If we're already running, initialize immediately.
            if (!_gottaStart)
            {
                _game.CollisionManager.RegisterCollider(c);
            }
        }

        internal void RunStart()
        {
            Start();
            // We might create colliders in start so register them now.
            _colliders.ForEach(c => _game.CollisionManager.RegisterCollider(c));
        }
        internal void RunUpdate(float dt)
        {
            AssertAlive();
            // Start before first tick
            EnsureStarted();
            Update(dt);
            UpdateCoroutines();
        }
        internal void RunPreUpdate(float dt)
        {
            AssertAlive();
            // Start before first tick
            EnsureStarted();
            PreUpdate(dt);
        }
        internal void RunPostUpdate(float dt)
        {
            AssertAlive();
            // Start before first tick
            EnsureStarted();
            PreUpdate(dt);
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

            // Deregister colliders
            _colliders.ForEach(c => _game.CollisionManager.DeregisterCollider(c));
            _colliders.Clear();

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

        #region Coroutine

        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            Coroutine c = new Coroutine(enumerator);
            _routines.Add(c);
            return c;
        }

        public void StopCoroutine(Coroutine c)
        {
            RemoveCoroutine(c);
        }

        public void StopAllCoroutines()
        {
            _routines.Clear();
        }

        internal void RemoveCoroutine(Coroutine c)
        {
            _routines.Remove(c);
        }

        // TODO: Pause & Resume coroutine for the extra spice

        private void UpdateCoroutines()
        {
            for (int i = _routines.Count - 1; i >= 0; --i)
            {
                if (!_routines[i].UpdateNext())
                {
                    _routines.RemoveAt(i);
                }
            }
            //_routines.RemoveAll(c => !c.UpdateNext());
        }

        #endregion

        #region Utils

        protected void EnsureStarted()
        {
            AssertAlive();
            if (_gottaStart)
            {
                RunStart();
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

        public virtual void PreUpdate(float dt)
        {

        }
        public virtual void PostUpdate(float dt)
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
