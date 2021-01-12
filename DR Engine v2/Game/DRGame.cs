using System;
using DREngine.Game.Controls;
using DREngine.Game.CoreScenes;
using DREngine.Game.UI;
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

        #region Static Hookups (For Modding!)
        public static Action<DRGame> GamePreInitialized; // Core stuff you want to initialize AT THE START
        public static Action<DRGame> GamePostInitialized; // Static systems that don't change per project. You can ignore this one if you want.
        public static Action<DRGame> GamePreUpdate;
        public static Action<DRGame> GamePostUpdate;
        public static Action<DRGame> GameProjectLoaded; // All UI overrides go here. ALl other overrides can go here also.
        #endregion

        public DRGame(string projectPath = null) : base("DR Game Test Draft")
        {
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
                GameData = ProjectData.LoadFromFile(path);

                ProjectPath = path;
                OnProjectInit();
                GameProjectLoaded?.Invoke(this);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not open project at path: {path}: {e.Message}");
                return false;
            }
        }

        public void InitializeEditorHookup(string editorPipeReadHandle, string editorPipeWriteHandle)
        {
            _editorConnection = new EditorConnection(true, editorPipeReadHandle, editorPipeWriteHandle);
        }

        public void InitializeEditorSceneTool(string sceneToEdit)
        {
            if (ProjectPath == null)
            {
                throw new InvalidOperationException("Project must be open when using scene editor mode!");
            }
            _sceneEditorScene = sceneToEdit;
        }

        #endregion

        #region Public Handlers

        public MenuControls MenuControls { get; }

        public ProjectData GameData = new ProjectData();
        public ResourceLoader ResourceLoader;

        public VNRunner VNRunner;

        public DRGameUI UI;

        public SaveState SaveState;

        #endregion

        #region Util variables

        public string ProjectPath = "";

        private EditorConnection _editorConnection;

        private readonly SplashScene _splashScene;
        private readonly ProjectMainMenuScene _projectMainMenuScene;

        private string _sceneEditorScene = null;
        
        #endregion

        #region Misc Util

        private void OnProjectInit()
        {
            if (_sceneEditorScene == null)
            {
                // Some of these depend on game project data.
                UI = new DRGameUI(this);
                SceneManager.LoadScene(_projectMainMenuScene);
            }
            else
            {
                Debug.LogDebug($"TO LOAD: {_sceneEditorScene}");
                // TODO: Load scene editor for given scene
                // SceneManager.LoadScene(new DREditableScene(_sceneEditorScene));
            }
        }
        #endregion

        #region Universal Game Loop

        protected override void Initialize()
        {
            GamePreInitialized?.Invoke(this);
            // Init data that should be available at the start.
            base.Initialize();

            //GameData.LoadDefaults();

            // Wait for editor if we need to.
            WaitForEditorConnection(() =>
            {
                LoadWhenSafe(() =>
                {
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
            GamePostInitialized?.Invoke(this);
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
            if (_editorConnection != null && _editorConnection.Active)
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
            GamePreUpdate?.Invoke(this);

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
                ProjectData.WriteToFile(new ProjectPath(this, "project.json"), GameData);
                //SaveState.Save(new ProjectPath(this, "TEST.save"));
            }
            else if (RawInput.KeyPressed(Keys.J))
            {
                Debug.Log("LOADING");
                GameData = ProjectData.LoadFromFile(new ProjectPath(this, "project.json"));
                //SaveState.Load(new ProjectPath(this, "TEST.save"));
            }


            // Open VN Script system
            VNRunner?.OnTick();

            // Update
            base.Update(gameTime);

            GamePostUpdate?.Invoke(this);
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