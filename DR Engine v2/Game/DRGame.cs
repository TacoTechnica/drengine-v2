using System;
using DREngine.Game.Controls;
using DREngine.Game.CoreScenes;
using DREngine.Game.VN;
using DREngine.ResourceLoading;
using GameEngine;
using GameEngine.Game;
using GameEngine.Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DREngine.Game
{
    public class DRGame : GamePlus
    {
        #region Constants

        private const string PROJECTS_DIRECTORY = "projects";

        #endregion

        public DRGame(string projectPath = null, bool connectToDebugEditor = false, string editorPipeReadHandle = "",
            string editorPipeWriteHandle = "") : base("DR Game Test Draft")
        {
            _editorConnection = new EditorConnection(connectToDebugEditor, editorPipeReadHandle, editorPipeWriteHandle);

            Graphics.SynchronizeWithVerticalRetrace = true;
            // Fixed timestep causes framerate issues, not sure why. Most likely will not set to true
            //this.IsFixedTimeStep = false;

            // For debugging UI
            Window.AllowUserResizing = true;

            ProjectPath = projectPath;

            // Init controls
            MenuControls = new MenuControls(this);

            // Init Core Scenes
            _splashScene = new SplashScene(this, PROJECTS_DIRECTORY);
            _projectMainMenuScene = new ProjectMainMenuScene(this);

            ResourceLoader = new ResourceLoader(ResourceLoaderData);
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
                ProjectPath = path;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not open project at path: {path}: {e.Message}");
                return false;
            }
        }

        #endregion

        #region Public Handlers

        public MenuControls MenuControls { get; }

        public ProjectData GameProjectData = new ProjectData();
        public ResourceLoader ResourceLoader;

        public VNRunner VNRunner;

        public SaveState SaveState;

        public Action OnPostUpdate;

        #endregion

        #region Util variables

        public string ProjectPath = "";

        private readonly EditorConnection _editorConnection;

        private readonly SplashScene _splashScene;
        private readonly ProjectMainMenuScene _projectMainMenuScene;

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
        ///     When there is no project this will run.
        /// </summary>
        private void LoadSplash()
        {
            SceneManager.LoadScene(_splashScene);
        }

        /// <summary>
        ///     When we wait on the editor for a connection, this will run.
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

            if (RawInput.KeyPressed(Keys.H))
            {
                Debug.Log("SAVING");
                ProjectData.WriteToFile(new ProjectPath(this, "project.json"), GameProjectData);
                //SaveState.Save(new ProjectPath(this, "TEST.save"));
            }
            else if (RawInput.KeyPressed(Keys.J))
            {
                Debug.Log("LOADING");
                GameProjectData = ProjectData.LoadFromFile(new ProjectPath(this, "project.json"));
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