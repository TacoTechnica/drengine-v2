using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game
{
    public class DRGame : GamePlus
    {
        #region Util variables

        // TODO: Make this changable depending on the game.
        private string WindowTitle = "DREngine Draft (Running No Game)";

        public ProjectData GameProjectData = null;

        #endregion


        public DRGame(string projectPath = null) : base("DR Game Test Draft", true, new TestGame())
        {

            this._graphics.SynchronizeWithVerticalRetrace = true;
            this.IsFixedTimeStep = false;
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
            LoadProject("projects/test_project");
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

            // Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        #endregion

    }
}
