using System;
using System.Diagnostics;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DREngine
{
    public class DRGame : Game
    {
#region Util variables

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private IGameStarter _starter = null;

        ///  Debug stuff
        private float _currentFPS = 0;
        private long _currentMemoryBytes = 0;
        private long _debugFrameCounter = 0;
        private Timer _debugTimer = new Timer();
        private DateTime _lastDebugTime;

        // TODO: Make this changable depending on the game.
        private string WindowTitle = "DREngine Draft (Running No Game)";

        public ProjectData GameProjectData = null;

#endregion

        private static DRGame _instance = null;
        public static DRGame Instance
        {
            get
            {
                if (_instance == null) Debug.LogError("No Engine Instance, everything will blow up");
                return _instance;
            }
        }

        public DRGame(string projectPath = null)
        {
            _instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.Window.Title = "Game Engine Test";
            this._graphics.SynchronizeWithVerticalRetrace = true;
            this.IsFixedTimeStep = false;

            _debugTimer.Interval = 1000f;
            _debugTimer.AutoReset = true;

            // Pick an implementation that you will use.
            _starter = new TestGame();
        }

#region Public Access

        public void LoadProject(string path)
        {
            try
            {
                ProjectData.ReadFromFile(path, out GameProjectData);
            }
            catch (Exception e)
            {
                ShowMessagePopup($"Could not open project at path: {path}: {e.Message}");
            }
        }

#endregion




#region Universal Game Loop

        protected override void Initialize()
        {
            // Init
            base.Initialize();

            // Start our sub-game
            Debug.LogDebug("DRGame Initialize()");
            _starter?.Initialize();

            // Start a debug timer for debug things.
            _debugTimer.Start();
            _lastDebugTime = DateTime.Now;
            _debugTimer.Elapsed += DebugTimerOnElapsed;

            LoadProject("projects/test_project");
        }

        protected override void LoadContent()
        {
            // Init load
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ++_debugFrameCounter;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update
            base.Update(gameTime);
            _starter?.Update(dt);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw
            base.Draw(gameTime);
            _starter?.Draw();
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
            float second_difference = (float) DateTime.Now.Subtract(_lastDebugTime).TotalSeconds;
            if (second_difference == 0) return;
            _currentFPS = (_debugFrameCounter / second_difference);
            _debugFrameCounter = 0;

            // Get current memory usage.
            Process proc = Process.GetCurrentProcess();
            _currentMemoryBytes = proc.PrivateMemorySize64;

            // Set window title
            Window.Title = $"{WindowTitle} | {_currentFPS:0.00} FPS | {((float)_currentMemoryBytes / (1000f*1000f)):0.00} mB";

            _lastDebugTime = DateTime.Now;
        }

#endregion
    }
}
