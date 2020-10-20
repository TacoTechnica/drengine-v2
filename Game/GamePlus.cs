using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using DREngine.Util;
using Gdk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DREngine.Game
{

    /// <summary>
    /// This is game functionality that is UNIVERSAL within this project.
    /// ALL games will be inherit from this game, and can in-theory
    /// make any kind of game that suits your needs.
    /// </summary>
    public class GamePlus : Microsoft.Xna.Framework.Game
    {
        #region Util variables

        protected GraphicsDeviceManager _graphics;

        private IGameRunner _runner = null;

        ///  Debug stuff
        private bool _debugTitle;
        private float _currentFPS = 0;
        private long _currentMemoryBytes = 0;
        private long _debugFrameCounter = 0;
        private float _fpsAverageAccum = 0;
        private Timer _debugTimer = new Timer();
        private DateTime _lastDebugTime;

        public float DebugMaxFPS = 60;

        internal BasicEffect DebugEffect;
        public bool DebugDrawColliders = false;

        #endregion

        #region Public Access and Handlers

        public string WindowTitle;

        public SpriteBatch SpriteBatch { get; private set; }

        public SceneManager SceneManager { get; private set; }

        public CollisionManager CollisionManager { get; private set; }

        /// Events
        public EventManager UpdateBegan { get; private set; } = new EventManager();
        public EventManager UpdateFinished { get; private set; } = new EventManager();

        public EventManager WhenSafeToLoad { get; private set; } = new EventManager();

        public float Time { get; private set; } = 0;
        public float TimeScale = 1f;
        public float UnscaledTime { get; private set; } = 0;
        public float DeltaTime { get; private set; } = 0;
        public float UnscaledDeltaTime { get; private set; } = 0;

        #endregion

        public GamePlus(string windowTitle = "Untitled Game", bool debugTitle = true, IGameRunner gameRunner = null)
        {
            WindowTitle = windowTitle;
            _runner = gameRunner;
            _debugTitle = debugTitle;

            // MonoGame config
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.Window.Title = windowTitle;

            // Debug config
            _debugTimer.Interval = 1000f;
            _debugTimer.AutoReset = true;

            SceneManager = new SceneManager(this);
            CollisionManager = new CollisionManager();
        }


        #region Universal Game Loop

        protected override void Initialize()
        {
            // Init
            base.Initialize();

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            _runner?.Initialize(this);

            // Start a debug timer for debug things.
            if (_debugTitle)
            {
                _debugTimer.Start();
                _lastDebugTime = DateTime.Now;
                _debugTimer.Elapsed += DebugTimerOnElapsed;
            }
            DebugEffect = new BasicEffect(GraphicsDevice);

            WhenSafeToLoad.InvokeAll();
        }

        protected override void Update(GameTime gameTime)
        {

            Input.UpdateState();

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UnscaledDeltaTime = DeltaTime;
            DeltaTime *= TimeScale;
            UnscaledTime += UnscaledDeltaTime;
            Time += DeltaTime;

            if (_debugTitle)
            {
                if (UnscaledDeltaTime != 0)
                {
                    ++_debugFrameCounter;
                    _fpsAverageAccum += 1f / UnscaledDeltaTime;
                }
            }

            UpdateBegan.InvokeAll();

            // TODO: Make this a bit more efficient I guess?
            SceneManager.GameObjects.LoopThroughAll(
                (obj) =>
                {
                    obj.RunPreUpdate(DeltaTime);
                }
            );

            SceneManager.GameObjects.LoopThroughAll(
                (obj) =>
                {
                    obj.RunUpdate(DeltaTime);
                }
            );

            // Update all update-able objects.
            SceneManager.GameObjects.LoopThroughAllAndDeleteQueued(
                (obj) =>
                {
                    obj.RunPostUpdate(DeltaTime);
                },
                (obj) =>
                {
                    obj.RunOnDestroy();
                }
            );

            // Update
            base.Update(gameTime);
            _runner?.Update(DeltaTime);

            WhenSafeToLoad.InvokeAll();
            UpdateFinished.InvokeAll();
        }

        protected override void Draw(GameTime gameTime)
        {

            // Draw all render-able objects.
            SceneManager.GameRenderObjects.LoopThroughAll((obj) =>
            {
                obj.PreDraw(_graphics.GraphicsDevice);
            });
            SceneManager.GameRenderObjects.LoopThroughAll((obj) =>
            {
                obj.Draw(_graphics.GraphicsDevice);
            });
            SceneManager.GameRenderObjects.LoopThroughAll((obj) =>
            {
                obj.PostDraw(_graphics.GraphicsDevice);
            });

            // Draw
            base.Draw(gameTime);
            _runner?.Draw();
        }

        #endregion

        #region Debug Util

        public void ShowMessagePopup(string message)
        {
            Debug.Log(message);
        }

        private void DebugTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            // Compute current FPS
            //float second_difference = (float) DateTime.Now.Subtract(_lastDebugTime).TotalSeconds;
            //if (second_difference == 0) return;
            //_currentFPS = (_debugFrameCounter / second_difference);
            if (_debugFrameCounter == 0) return;
            _currentFPS = MathF.Min(_fpsAverageAccum / _debugFrameCounter, DebugMaxFPS);
            _fpsAverageAccum = 0;
            _debugFrameCounter = 0;

            // Get current memory usage.
            Process proc = Process.GetCurrentProcess();
            _currentMemoryBytes = proc.PrivateMemorySize64;

            int objs = SceneManager.GameObjects.Count;
            int rends = SceneManager.GameRenderObjects.Count;

            // Set window title
            Window.Title = $"{WindowTitle} | {_currentFPS:0.00} FPS | {((float)_currentMemoryBytes / (1000f*1000f)):0.00} mB | {objs} Objects, {rends} Renderers";

            _lastDebugTime = DateTime.Now;
        }

        #endregion
    }
}
