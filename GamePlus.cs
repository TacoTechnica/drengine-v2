using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using DREngine.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DREngine
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

        private IGameStarter _starter = null;

        ///  Debug stuff
        private bool _debugTitle;
        private float _currentFPS = 0;
        private long _currentMemoryBytes = 0;
        private long _debugFrameCounter = 0;
        private Timer _debugTimer = new Timer();
        private DateTime _lastDebugTime;

        public string WindowTitle;

        private LinkedList<Camera3D> _cameras = new LinkedList<Camera3D>();

#endregion


        public GamePlus(string windowTitle = "Untitled Game", bool debugTitle = true, IGameStarter gameStarter = null)
        {
            WindowTitle = windowTitle;
            _starter = gameStarter;
            _debugTitle = debugTitle;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.Window.Title = windowTitle;

            _debugTimer.Interval = 1000f;
            _debugTimer.AutoReset = true;
        }

#region Public Access

        // TODO: Get cameras by layer.
        public IEnumerable<Camera3D> GetCameras()
        {
            return _cameras;
        }

        /// Camera access. CAMERA USE ONLY
        public LinkedListNode<Camera3D> AddInternalCamera(Camera3D camera)
        {
            return _cameras.AddLast(camera);
        }
        public void RemoveInternalCamera(LinkedListNode<Camera3D> camnode)
        {
            _cameras.Remove(camnode);
        }

#endregion




#region Universal Game Loop

        protected override void Initialize()
        {
            // Init
            base.Initialize();

            _starter?.Initialize(this);

            // Start a debug timer for debug things.
            if (_debugTitle)
            {
                _debugTimer.Start();
                _lastDebugTime = DateTime.Now;
                _debugTimer.Elapsed += DebugTimerOnElapsed;
            }
        }


        protected override void Update(GameTime gameTime)
        {
            if (_debugTitle) ++_debugFrameCounter;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update
            base.Update(gameTime);
            _starter?.Update(dt);
        }

        protected override void Draw(GameTime gameTime)
        {
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
