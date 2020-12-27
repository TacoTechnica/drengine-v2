﻿using System;
using DREngine.Game.Controls;
using DREngine.Game.CoreScenes;
using DREngine.Game.VN;
using GameEngine.Game;
using GameEngine.Game.Input;
using GameEngine.Test;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Debug = GameEngine.Debug;

namespace DREngine.Game
{
    public class DRGame : GamePlus
    {

        #region Constants

        private const string PROJECTS_DIRECTORY = "projects";

        #endregion

        #region Public Handlers

        public MenuControls MenuControls { get; private set; }

        public ProjectData GameProjectData = new ProjectData();
        public ProjectResources ProjectResources;

        public VNRunner VNRunner;

        public SaveState SaveState;

        public Action OnPostUpdate;

        #endregion

        #region Util variables


        public string ProjectPath = "";

        private SplashScene SplashScene;
        private ProjectMainMenuScene _projectMainMenuScene;

        #endregion

        public DRGame(string projectPath = null) : base("DR Game Test Draft", "Content", true)
        {
            this._graphics.SynchronizeWithVerticalRetrace = true;
            // Fixed timestep causes framerate issues, not sure why. Most likely will not set to true
            //this.IsFixedTimeStep = false;

            // For debugging UI
            this.Window.AllowUserResizing = true;

            ProjectPath = projectPath;

            // Init controls
            MenuControls = new MenuControls(this);

            // Init Core Scenes
            SplashScene = new SplashScene(this, PROJECTS_DIRECTORY);
            _projectMainMenuScene = new ProjectMainMenuScene(this);

            ProjectResources = new ProjectResources(this);
            ProjectResourceConverter.OnInitGame(this);

            VNRunner = new VNRunner(this);

            SaveState = new SaveState(this);

        }

        #region Public Access

        public bool LoadProject(string path)
        {
            try
            {
                Debug.LogDebug($"Loading Project at {path}");
                GameProjectData = ProjectData.LoadFromFile(path);
                SceneManager.LoadScene(_projectMainMenuScene);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not open project at path: {path}: {e.Message}");
                return false;
            }
        }

        #endregion

        #region Universal Game Loop

        protected override void Initialize()
        {
            // Init data that should be available at the start.
            base.Initialize();

            GameProjectData.LoadDefaults();

            Debug.LogDebug("DRGame Initialize()");
            if (ProjectPath != null && LoadProject(ProjectPath))
            {
                Debug.LogDebug($"Loaded Project at {ProjectPath}");
            }
            else
            {
                Debug.LogDebug($"Project \"{ProjectPath}\" either not specified or invalid. Going to Splash Screen.");
                LoadSplash();
            }
        }

        /// <summary>
        /// When there is no project this will run.
        /// </summary>
        private void LoadSplash()
        {
            SceneManager.LoadScene(SplashScene);
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

            if (RawInput.KeyPressed(Keys.NumPad0))
            {
                SaveState.Save(new ProjectPath(this, "TEST.save"));
            } else if (RawInput.KeyPressed(Keys.NumPad1))
            {
                SaveState.Load(new ProjectPath(this, "TEST.save"));
            }


            // Update
            base.Update(gameTime);

            OnPostUpdate?.Invoke();
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
