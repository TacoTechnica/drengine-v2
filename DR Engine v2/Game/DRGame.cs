using System;
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
        public ResourceLoader ResourceLoader;

        public VNRunner VNRunner;

        public SaveState SaveState;

        public Action OnPostUpdate;

        #endregion

        #region Util variables

        public string ProjectPath = "";

        private EditorConnection _editorConnection;

        private SplashScene SplashScene;
        private ProjectMainMenuScene _projectMainMenuScene;

        #endregion

        public DRGame(string projectPath = null, bool connectToDebugEditor = false, string editorPipeReadHandle = "", string editorPipeWriteHandle = "") : base("DR Game Test Draft", "Content", true)
        {
            _editorConnection = new EditorConnection(connectToDebugEditor, editorPipeReadHandle, editorPipeWriteHandle);
            
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

            ResourceLoader = new ResourceLoader(this);
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

            //GameProjectData.LoadDefaults();

            // Wait for editor if we need to.
            WaitForEditorConnection(() =>
            {
                LoadWhenSafe(() =>
                {
                    Debug.LogDebug("DRGame Initialize()");
                    if (ProjectPath != null && LoadProject(ProjectPath))
                    {
                        Debug.LogDebug($"Loaded Project at {ProjectPath}");
                    }
                    else
                    {
                        Debug.LogDebug(
                            $"Project \"{ProjectPath}\" either not specified or invalid. Going to Splash Screen.");
                        LoadSplash();
                    }
                });
            });

        }

        /// <summary>
        /// When there is no project this will run.
        /// </summary>
        private void LoadSplash()
        {
            SceneManager.LoadScene(SplashScene);
        }

        /// <summary>
        /// When we wait on the editor for a connection, this will run.
        /// </summary>
        private void WaitForEditorConnection(Action onPing)
        {
            // Wait for editor if we need to. Otherwise, immediately start.
            if (_editorConnection.Active)
            {
                SceneManager.LoadScene(new EditorConnectionScene(this));
                _editorConnection.BeginReceiving();
                _editorConnection.WaitOnEditorPingAsync(onPing);
            }
            else
            {
                //SceneManager.LoadScene(new EditorConnectionScene(this));
                onPing?.Invoke();
            }
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
                Debug.Log("SAVING");
                ProjectData.WriteToFile(new ProjectPath(this, "project.json"), GameProjectData );
                //SaveState.Save(new ProjectPath(this, "TEST.save"));
            } else if (RawInput.KeyPressed(Keys.NumPad1))
            {
                Debug.Log("LOADING");
                GameProjectData = ProjectData.LoadFromFile(new ProjectPath(this,"project.json"));
                //SaveState.Load(new ProjectPath(this, "TEST.save"));
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
