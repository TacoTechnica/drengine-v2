using System;
using System.Collections;
using System.Collections.Generic;
using DREngine.Game.Tween;

namespace DREngine.Game
{
    public abstract class GameObject : IGameObject
    {
        protected GamePlus _game;

        private bool _gottaStart = true;

        public bool Active { get; private set; }
        protected internal bool _activeAsChild { get; private set; } // Whether we should automatically get activated as a child.
        protected internal bool _parentActive { get; private set; }

        private ObjectContainerNode<GameObject> _gameAddedNode = null;

        private ObjectContainer<GameObject> _children = new ObjectContainer<GameObject>();

        // To catch circular parenting which would break the game badly.
        private HashSet<GameObject> _parents = new HashSet<GameObject>();

        private bool _alive = false;

        private List<Coroutine> _routines = new List<Coroutine>();
        private List<ICollider> _colliders = new List<ICollider>();

        public Tweener Tweener { get; private set; }

        public GameObject(GamePlus game)
        {
            _game = game;
            _gameAddedNode = game.SceneManager.GameObjects.Add(this);

            _gottaStart = true;
            _alive = true;
            Active = true;
            _activeAsChild = true;
            _parentActive = true;

            Tweener = NewTweener(game);

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
            _children.Add(obj);
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

        /// <summary>
        /// Set the object to be active.
        /// </summary>
        /// <param name="active"> Whether to set it to active. </param>
        /// <param name="childMode"> If true, will NOT activate child objects that were disabled previously. </param>
        public void SetActive(bool active, bool childMode=false)
        {

            if (childMode)
            {
                _parentActive = active;
                // If we're a child, keep ourselves de-activated if we were activated previously.
                if (active == Active || (active && _activeAsChild) == Active) return;
                Active = active && _activeAsChild;
            }
            else
            {
                if (active == Active) return;
                _activeAsChild = active;
                // Do nothing while our parent isn't active.
                if (!_parentActive) return;
                Active = active;
            }

            if (Active)
            {
                _game.SceneManager.GameObjects.EnableEnqueue(_gameAddedNode);
            }
            else
            {
                _game.SceneManager.GameObjects.DisableEnqueue(_gameAddedNode);
            }

            // Do the same for the children
            _children.LoopThroughAll(child =>
            {
                child.SetActive(Active, true);
            });
        }

        internal void RunStart()
        {
            if (Active)
            {
                RunOnEnable(_gameAddedNode);
            }

            Start();
            // We might create colliders in start so register them now.
            _colliders.ForEach(c => _game.CollisionManager.RegisterCollider(c));
        }
        internal void RunUpdate(float dt)
        {
            AssertAlive();
            // Start before first tick
            EnsureStarted();
            Tweener?.RunUpdate();
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

            Tweener.CancelAll();

            _gameAddedNode = null;

            // Delete all children too.
            _children.LoopThroughAll(child =>
            {
                child.Destroy();
            });
            // Cleanup, may as well empty the list.
            _children.RemoveAllQueuedImmediate();

            // Kinda redundant but good to clean up.
            _parents.Clear();

            // Deregister colliders
            _colliders.ForEach(c => _game.CollisionManager.DeregisterCollider(c));
            _colliders.Clear();

            OnDestroy();

            if (Active)
            {
                RunOnDisable(_gameAddedNode);
            }

            _alive = false;
        }

        internal virtual void RunOnEnable(ObjectContainerNode<GameObject> newNode)
        {
            _gameAddedNode = newNode;
            OnEnable();
        }
        internal virtual void RunOnDisable(ObjectContainerNode<GameObject> newNode)
        {
            _gameAddedNode = newNode;
            OnDisable();
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
            _game.SceneManager.GameObjects.RemoveImmediate(_gameAddedNode, (self) => { self.RunOnDestroy(); });
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

        protected virtual Tweener NewTweener(GamePlus game)
        {
            return new Tweener(game);
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

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
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
