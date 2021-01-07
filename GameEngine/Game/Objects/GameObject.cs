using System;
using System.Collections;
using System.Collections.Generic;
using GameEngine.Game.Collision;
using GameEngine.Game.Coroutine;
using GameEngine.Game.Tween;
using Newtonsoft.Json;

namespace GameEngine.Game.Objects
{
    public abstract class GameObject : IGameObject
    {
        private bool _alive;

        private readonly ObjectContainer<GameObject> _children = new ObjectContainer<GameObject>();

        private readonly List<ICollider> _colliders = new List<ICollider>();

        private readonly CoroutineRunner _coroutineRunner = new CoroutineRunner();
        protected readonly GamePlus Game;

        private ObjectContainerNode<GameObject> _gameAddedNode;

        private bool _gottaStart = true;

        // To catch circular parenting which would break the game badly.
        private readonly HashSet<GameObject> _parents = new HashSet<GameObject>();

        public GameObject(GamePlus game)
        {
            Game = game;
            _gameAddedNode = game.SceneManager.GameObjects.Add(this);

            _gottaStart = true;
            _alive = true;
            Active = true;
            ActiveAsChild = true;
            ParentActive = true;

            // ReSharper disable once VirtualMemberCallInConstructor
            Tweener = NewTweener(game);

            // ReSharper disable once VirtualMemberCallInConstructor
            Awake();
        }

        [JsonIgnore] public bool Active { get; private set; }

        protected internal bool
            ActiveAsChild { get; private set; } // Whether we should automatically get activated as a child.

        protected internal bool ParentActive { get; private set; }

        [JsonIgnore] public Tweener Tweener { get; }

        // When we've been destroyed, we can be compared to null.
        public static bool operator ==(GameObject obj, object other)
        {
            // Ensure not null
            if (obj is { } && obj._alive)
                return obj.Equals(other);
            return Equals(other, null);
        }

        public static bool operator !=(GameObject obj, object other)
        {
            return !(obj == other);
        }

        #region Public Control

        public void AddChild(GameObject obj)
        {
            AssertAlive();
            if (obj._parents.Contains(this))
                throw new InvalidOperationException(
                    $"Attempted to add child to parent more than once! Child: {obj}, Parent: {this}");

            if (_parents.Contains(obj))
                throw new InvalidOperationException(
                    $"Circular Parenting Detected when trying to add child! Child: {obj}, Parent: {this}");
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
            if (!_gottaStart) Game.CollisionManager.RegisterCollider(c);
        }

        /// <summary>
        ///     Set the object to be active.
        /// </summary>
        /// <param name="active"> Whether to set it to active. </param>
        /// <param name="childMode"> If true, will NOT activate child objects that were disabled previously. </param>
        public void SetActive(bool active, bool childMode = false)
        {
            if (childMode)
            {
                ParentActive = active;
                // If we're a child, keep ourselves de-activated if we were activated previously.
                if (active == Active || (active && ActiveAsChild) == Active) return;
                Active = active && ActiveAsChild;
            }
            else
            {
                if (active == Active) return;
                ActiveAsChild = active;
                // Do nothing while our parent isn't active.
                if (!ParentActive) return;
                Active = active;
            }

            if (Active)
                Game.SceneManager.GameObjects.EnableEnqueue(_gameAddedNode);
            else
                Game.SceneManager.GameObjects.DisableEnqueue(_gameAddedNode);

            // Do the same for the children
            _children.LoopThroughAll(child => { child.SetActive(Active, true); });
        }

        internal void RunStart()
        {
            if (Active) RunOnEnable(_gameAddedNode);

            Start();
            // We might create colliders in start so register them now.
            _colliders.ForEach(c => Game.CollisionManager.RegisterCollider(c));
        }

        internal void RunUpdate(float dt)
        {
            AssertAlive();
            // Start before first tick
            EnsureStarted();
            Tweener?.RunUpdate();
            Update(dt);
            _coroutineRunner.OnTick();
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
            _children.LoopThroughAll(child => { child.Destroy(); });
            // Cleanup, may as well empty the list.
            _children.RemoveAllQueuedImmediate();

            // Kinda redundant but good to clean up.
            _parents.Clear();

            // Deregister colliders
            _colliders.ForEach(c => Game.CollisionManager.DeregisterCollider(c));
            _colliders.Clear();

            OnDestroy();

            if (Active) RunOnDisable(_gameAddedNode);

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
            Game.SceneManager.GameObjects.RemoveEnqueue(_gameAddedNode);
        }

        public void DestroyImmediate()
        {
            AssertAlive();
            if (!_alive) return;
            Game.SceneManager.GameObjects.RemoveImmediate(_gameAddedNode, self => { self.RunOnDestroy(); });
        }

        #endregion

        #region Coroutine

        public Coroutine.Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return _coroutineRunner.Run(enumerator);
        }

        public void StopCoroutine(Coroutine.Coroutine c)
        {
            RemoveCoroutine(c);
        }

        public void StopAllCoroutines()
        {
            _coroutineRunner.StopAll();
        }

        internal void RemoveCoroutine(Coroutine.Coroutine c)
        {
            _coroutineRunner.Stop(c);
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
                throw new InvalidOperationException(
                    $"Object is destroyed but you still tried accessing it! Object: {this}");
        }

        protected virtual Tweener NewTweener(GamePlus game)
        {
            return new Tweener(game);
        }

        #endregion

        #region Interface

        /// <summary>
        /// </summary>
        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"> delta time in seconds </param>
        public virtual void Update(float dt)
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
    }
}