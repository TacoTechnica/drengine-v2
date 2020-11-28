using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Timers;
using GameEngine.Game.Audio;
using GameEngine.Game.Debugging;
using GameEngine.Game.Debugging.CommandListGlobal;
using GameEngine.Game.Input;
using GameEngine.Game.Resources;
using GameEngine.Game.UI;
using GameEngine.Util;
using Gdk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using Cursor = GameEngine.Game.Input.Cursor;

namespace GameEngine.Game
{

    /// <summary>
    /// This is game functionality that is UNIVERSAL within this project.
    /// ALL games will be inherit from this game, and can in-theory
    /// make any kind of game that suits your needs.
    /// </summary>
    public class GamePlus : Microsoft.Xna.Framework.Game
    {
        #region Util variables & Debug

        protected GraphicsDeviceManager _graphics;

        private IGameRunner _runner = null;

        ///  Debug stuff
        private bool _debug;
        private float _currentFPS = 0;
        private long _currentMemoryBytes = 0;
        private long _debugFrameCounter = 0;
        private float _fpsAverageAccum = 0;
        private float _fpsMin = float.PositiveInfinity;
        private Timer _debugTimer = new Timer();
        private Stopwatch _frameTimer = new Stopwatch();
        private long _lastFrameInterval;
        private DateTime _lastDebugTime;
        private DebugControls _debugControls;
        private const string GLOBAL_DEBUG_COMMAND_NAMESPACE = "DREngine.Game.Debugging.CommandListGlobal";

        public DebugConsole DebugConsole;

        internal BasicEffect DebugEffect;
        public bool DebugDrawColliders = false;
        public SpriteBatch DebugSpriteBatch;

        private List<Controls> _controls = new List<Controls>();

        private GenericCursor _cursor = new GenericCursor();

        private bool _safeToLoad = false;

        #endregion

        #region Public Handlers and Variables

        public string WindowTitle;

        /// Audio
        public AudioOutput AudioOutput { get; private set; }

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
        public UIScreen UiScreen { get; private set; }

        /// Scene Object Manager
        public SceneManager SceneManager { get; private set; }

        /// Collision Manager
        public CollisionManager CollisionManager { get; private set; }

        /// Events
        public EventManager UpdateBegan { get; private set; } = new EventManager();
        public EventManager UpdateFinished { get; private set; } = new EventManager();

        private EventManager _whenSafeToLoad = new EventManager(true);// { get; private set; } = new EventManager();

        /// Time
        public float Time { get; private set; } = 0;
        public float TimeScale = 1f;
        public float UnscaledTime { get; private set; } = 0;
        public float DeltaTime { get; private set; } = 0;
        public float UnscaledDeltaTime { get; private set; } = 0;

        #endregion

        public GamePlus(string windowTitle = "Untitled Game", string contentPath = "Content", bool debug = true, IGameRunner gameRunner = null)
        {
            WindowTitle = windowTitle;
            _runner = gameRunner;
            _debug = debug;

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);

            // MonoGame config
            _graphics = new GraphicsDeviceManager(this)
            {
                // Add a depth stencil buffer
                //PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };
            _graphics.SynchronizeWithVerticalRetrace = false; //Vsync
            Content.RootDirectory = contentPath;
            IsMouseVisible = true;
            Window.Title = windowTitle;

            RawInput.SetGame(this);

            SceneManager = new SceneManager(this);
            CollisionManager = new CollisionManager();
            UiScreen = new UIScreen(this);

            AudioOutput = new AudioOutput();


            // Debug config
            if (debug)
            {
                _debugTimer.Interval = 1000f;
                _debugTimer.AutoReset = true;
                _debugControls = new DebugControls(this);
                // Initialize commands
                Commands.Init(typeof(Help));
            }
        }


        #region Public Access

        public void LoadWhenSafe(Action onSafeToLoad)
        {
            if (!_safeToLoad)
            {
                _whenSafeToLoad.AddListener(onSafeToLoad);
            }
            else
            {
                onSafeToLoad.Invoke();
            }
        }

        #endregion

        #region Universal Game Loop

        protected override void Initialize()
        {
            // Init
            base.Initialize();

            UiScreen.Initialize();

            // Initialize Debug Stuff
            if (_debug)
            {
                InitializeDebug();
            }
            DebugEffect = new BasicEffect(GraphicsDevice);
            DebugSpriteBatch = new SpriteBatch(GraphicsDevice);

            _whenSafeToLoad.InvokeAll();
            _safeToLoad = true;

            _runner?.Initialize(this);
        }

        private void InitializeDebug()
        {
            _debugTimer.Start();
            _lastDebugTime = DateTime.Now;
            _debugTimer.Elapsed += DebugTimerOnElapsed;
            Font debugFont = null;
            string[] pathsToTry =
            {
                @"C:\\Windows\\Fonts\lucon.ttf",
                @"C:\\Windows\\Fonts\arial.ttf",
                "/usr/share/fonts/TTF/consola.ttf",
                "/usr/share/fonts/TTF/arial.ttf"
            };
            bool foundValid = false;
            foreach (string path in pathsToTry)
            {
                Debug.LogDebug($"Trying {path}...");
                try
                {
                    debugFont = new Font(this, path, 16);
                    foundValid = true;
                }
                catch (Exception)
                {
                    Debug.LogDebug("(Failed)");
                    continue;
                }

                break;
            }

            if (!foundValid)
            {
                throw new ContentLoadException($"Failed to load default debug font from: {pathsToTry}. " +
                                               "Either add a font to the content pipeline at \"Debug/DebugFont\" or " +
                                               "make sure one of the tried system fonts is available.");
            }
            DebugConsole = new DebugConsole(this,
                debugFont,
                256f,
                _debugControls.ConsoleOpen, _debugControls.ConsoleClose, _debugControls.ConsoleSubmit
            );
        }

        protected override void Update(GameTime gameTime)
        {
            long elapsed = _lastFrameInterval;

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
            UiScreen.Update();

            // If we're debugging, handle that right off the bat.
            if (_debug)
            {
                if (elapsed != 0)
                {
                    float deltaSeconds = (float) elapsed / 1000f;
                    ++_debugFrameCounter;
                    float currentFPS = 1f / deltaSeconds;
                    _fpsAverageAccum += currentFPS;
                    if (currentFPS < _fpsMin)
                    {
                        _fpsMin = currentFPS;
                    }
                }
            }

            // Update Inputs
            foreach (Controls c in _controls)
            {
                c.DoUpdate();
            }

            // Resources that want to load at the START may load now.
            //_whenSafeToLoad.InvokeAll();

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
            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;

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
            UiScreen.Draw();

            _frameTimer.Stop();
            _lastFrameInterval = _frameTimer.ElapsedMilliseconds;
            //Debug.Log($"OOF: {_frameTimer.ElapsedMilliseconds}");
            _frameTimer.Restart();

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
            float maxFPS = 1000f / TargetElapsedTime.Milliseconds;
            _currentFPS = MathF.Min(_fpsAverageAccum / _debugFrameCounter, maxFPS);
            _fpsAverageAccum = 0;
            _debugFrameCounter = 0;

            // Get current memory usage.
            Process proc = Process.GetCurrentProcess();
            _currentMemoryBytes = proc.PrivateMemorySize64;

            // Get object and UI counts
            int objs = SceneManager.GameObjects.Count;
            int rends = SceneManager.GameRenderObjects.Count;
            int uiActive = UiScreen.GetActiveUICountAfterDraw();
            int uiTotal = UiScreen.GetTotalUICountAfterDraw();

            // Set window title
            Window.Title = $"{WindowTitle} | {_currentFPS:0.00} FPS ({_fpsMin:0.00} Worst) | {((float)_currentMemoryBytes / (1000f*1000f)):0.00} mB | {objs} Objects, {rends} Renderers, {uiActive} / {uiTotal} UI";

            _fpsMin = float.PositiveInfinity;
            _lastDebugTime = DateTime.Now;
        }

        #endregion
    }
}
