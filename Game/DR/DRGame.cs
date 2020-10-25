using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using DREngine.Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace DREngine.Game
{
    public class DRGame : GamePlus
    {
        #region Util variables

        public ProjectData GameProjectData = new ProjectData();

        #endregion

        public DRGame(string projectPath = null) : base("DR Game Test Draft", true, new TestVectorFont())
        {
            this._graphics.SynchronizeWithVerticalRetrace = true;
            this.IsFixedTimeStep = false;

            // For debugging UI
            this.Window.AllowUserResizing = true;
        }

        #region Public Access

        public void LoadProject(string path)
        {
            try
            {
                GameProjectData = ProjectData.ReadFromFile(GraphicsDevice, path);
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
            // Init data that should be available at the start.
            Debug.Log("(pre init)");
            GameProjectData.LoadDefaults(GraphicsDevice);

            base.Initialize();

            // TODO: Loading project should use the defined project path, or if not defined it will load defaults.
            Debug.LogDebug("DRGame Initialize()");
            LoadProject("projects/test_project");
            // GameProjectData.LoadDefaults();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
