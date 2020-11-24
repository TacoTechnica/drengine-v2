using System;
using DREngine.Game.Controls;
using GameEngine.Game;
using GameEngine.Game.Input;
using GameEngine.Test;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Debug = GameEngine.Debug;

namespace DREngine.Game
{
    public class DRGame : GamePlus
    {

        #region Public Handlers

        public MenuControls MenuControls { get; private set; }

        #endregion

        #region Util variables

        public ProjectData GameProjectData = new ProjectData();

        private string _projectPath = null;

        #endregion

        public DRGame(string projectPath = null) : base("DR Game Test Draft", "Content", true, new TestMouseCollider())
        {
            this._graphics.SynchronizeWithVerticalRetrace = true;
            this.IsFixedTimeStep = false;

            // For debugging UI
            this.Window.AllowUserResizing = true;

            _projectPath = projectPath;

            // Init controls
            MenuControls = new MenuControls(this);
        }

        #region Public Access

        public bool LoadProject(string path)
        {
            try
            {
                GameProjectData = ProjectData.ReadFromFile(GraphicsDevice, path);
                return true;
            }
            catch (Exception e)
            {
                ShowMessagePopup($"Could not open project at path: {path}: {e.Message}");
                return false;
            }
        }

        #endregion

        #region Universal Game Loop

        protected override void Initialize()
        {
            // Init data that should be available at the start.
            Debug.Log("(pre init)");
            GameProjectData.LoadDefaults(GraphicsDevice);

            base.Initialize();

            Debug.LogDebug("DRGame Initialize()");
            if (_projectPath != null && LoadProject(_projectPath))
            {
                // Success!
            }
            else
            {
                LoadSplash();
            }
        }

        /// <summary>
        /// When there is no project this will run.
        /// </summary>
        private void LoadSplash()
        {

        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            /*
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            */

            if (RawInput.KeyPressed(Keys.F1))
            {
                Debug.LogDebug("Toggling Debug Collider Drawing");
                DebugDrawColliders = !DebugDrawColliders;
            }


            // Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Ah classic
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        #endregion

    }
}
