using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using DREngine.Game.Input;
using DREngine.Game.UI;
using DREngine.Util;
using Gdk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Cursor = DREngine.Game.Input.Cursor;

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
        public SpriteBatch DebugSpriteBatch;

        private List<Controls> _controls = new List<Controls>();

        private GenericCursor _cursor = new GenericCursor();

        #endregion

        #region Public Access and Handlers

        public string WindowTitle;

        /// Cursor
        public GenericCursor CurrentCursor
        {
            get => _cursor;
            set
            {
                if (value == null) throw new InvalidOperationException("Can't set our current cursor to null!");
                _cursor = value;
            }
        }

        /// Screen UI Renderer
        public UIScreen UIScreen { get; private set; }

        /// Scene Object Manager
        public SceneManager SceneManager { get; private set; }

        /// Collision Manager
        public CollisionManager CollisionManager { get; private set; }

        /// Events
        public EventManager UpdateBegan { get; private set; } = new EventManager();
        public EventManager UpdateFinished { get; private set; } = new EventManager();

        public EventManager WhenSafeToLoad { get; private set; } = new EventManager();

        /// Time
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
            UIScreen = new UIScreen(this);
        }


        #region Universal Game Loop

        protected override void Initialize()
        {
            // Init
            base.Initialize();

            // Start a debug timer for debug things.
            if (_debugTitle)
            {
                _debugTimer.Start();
                _lastDebugTime = DateTime.Now;
                _debugTimer.Elapsed += DebugTimerOnElapsed;
            }
            DebugEffect = new BasicEffect(GraphicsDevice);
            DebugSpriteBatch = new SpriteBatch(GraphicsDevice);

            UIScreen.Initialize();

            WhenSafeToLoad.InvokeAll();

            _runner?.Initialize(this);
        }

        protected override void Update(GameTime gameTime)
        {
            // Update input First.
            RawInput.UpdateState();

            // Perform delta time calculations
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UnscaledDeltaTime = DeltaTime;
            DeltaTime *= TimeScale;
            UnscaledTime += UnscaledDeltaTime;
            Time += DeltaTime;

            // Update cursor after we know our current delta times.
            CurrentCursor?.DoUpdate(this);

            // Update our UI screen. This will handle stuff we don't call for every draw.
            UIScreen.Update();

            // If we're debugging, handle that right off the bat.
            if (_debugTitle)
            {
                if (UnscaledDeltaTime != 0)
                {
                    ++_debugFrameCounter;
                    _fpsAverageAccum += 1f / UnscaledDeltaTime;
                }
            }

            // Update Inputs
            foreach (Controls c in _controls)
            {
                c.DoUpdate();
            }

            // Resources that want to load at the START may load now.
            WhenSafeToLoad.InvokeAll();

            // If any objects were Enabled LAST frame, make sure our list reflects that.
            SceneManager.GameObjects.EnableAllQueuedImmediate((obj, node) =>
            {
                obj.RunOnEnable(node);
            });

            // Everyone else can start on this frame.
            UpdateBegan.InvokeAll();

            // TODO: Maybe avoid scrolling through objects that don't use preupdate.
            // TODO: I moved the deletion up here instead of in post, verify this and delete this comment if everything is OK.
            SceneManager.GameObjects.LoopThroughAllAndDeleteQueued(
                (obj) =>
                {
                    obj.RunPreUpdate(DeltaTime);
                },
                (obj) =>
                {
                    obj.RunOnDestroy();
                }
            );

            SceneManager.GameObjects.LoopThroughAll(
                (obj) =>
                {
                    obj.RunUpdate(DeltaTime);
                }
            );

            // The last one will also go through and handle queued deletions.
            SceneManager.GameObjects.LoopThroughAll(
                (obj) =>
                {
                    obj.RunPostUpdate(DeltaTime);
                }
            );

            // Standard Update goes here.
            base.Update(gameTime);
            _runner?.Update(DeltaTime);

            // If any objects were disabled this frame, make sure our list reflects that.
            SceneManager.GameObjects.DisableAllQueuedImmediate((obj, node) =>
            {
                obj.RunOnDisable(node);
            });

            // We've finished the update frame.
            UpdateFinished.InvokeAll();
        }

        protected override void Draw(GameTime gameTime)
        {
            // Set Default Graphics
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

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

            // Draw UI
            UIScreen.Draw();
        }

        #endregion

        #region Debug Util

        public void AddControls(Controls c)
        {
            _controls.Add(c);
        }

        public void RemoveControls(Controls c)
        {
            _controls.Remove(c);
        }

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
