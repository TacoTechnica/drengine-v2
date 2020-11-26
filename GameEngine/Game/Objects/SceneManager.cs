using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Game.UI;

namespace GameEngine.Game
{
    /// <summary>
    /// Manages the current scene.
    /// </summary>
    public class SceneManager
    {

        private GamePlus _game;

        /// Object Containers
        internal ObjectContainer<GameObject> GameObjects { get; private set; }
        internal ObjectContainer<GameObjectRender> GameRenderObjects { get; private set; }
        internal ObjectContainer<Camera3D> Cameras { get; private set; }

        /// Loader Data
        private Dictionary<string, ISceneLoader> _sceneLoaderNameMap = new Dictionary<string, ISceneLoader>();

        private List<ISceneLoader> _sceneLoaders = new List<ISceneLoader>();

        // Loading util
        private ISceneLoader _toLoadNext = null;

        public int RegisteredSceneCount
        {
            get
            {
                return _sceneLoaders.Count;
            }
        }

        public SceneManager(GamePlus game)
        {
            _game = game;
            // Tracked objects
            GameObjects = new ObjectContainer<GameObject>();
            GameRenderObjects = new ObjectContainer<GameObjectRender>();
            Cameras = new ObjectContainer<Camera3D>();
        }

        /// <summary>
        /// Add a scene loader that we can reference to load a scene.
        /// Also assigns a unique ID to said loader.
        /// </summary>
        /// <param name="loader"> The loader to register. </param>
        /// <returns> A unique ID that the loader can use. </returns>
        internal int RegisterSceneLoader(ISceneLoader loader)
        {
            int id = _sceneLoaders.Count;
            _sceneLoaders.Add(loader);
            foreach (string name in loader.GetNames())
            {
                _sceneLoaderNameMap.Add(name, loader);
            }

            return id;
        }

        internal void DeregisterSceneLoader(ISceneLoader loader)
        {
            _sceneLoaders.Remove(loader);
            foreach (string name in loader.GetNames())
            {
                _sceneLoaderNameMap.Remove(name);
            }
        }

        /// <summary>
        /// Does scene with `name` exist? AKA, Can scene `name` be loaded?
        /// </summary>
        public bool SceneExists(string name)
        {
            return _sceneLoaderNameMap.ContainsKey(name);
        }

        /// <summary>
        /// Load a scene given its name. Scene name must be valid, check with SceneExists beforehand.
        /// </summary>
        /// <param name="name"> The name of the scene to load. </param>
        /// <exception cref="ArgumentException"> Thrown when the `name` is not a valid registered scene. </exception>
        public void LoadScene(string name)
        {
            if (!SceneExists(name))
                throw new ArgumentException($"Scene Loader with name {name} does not exist! If this is thrown within the editor this error should have been caught!!");

            LoadScene(_sceneLoaderNameMap[name]);
        }

        /// <summary>
        /// Load a scene given its unique id. Scene `id` must be valid.
        /// </summary>
        /// <param name="id"> The id of the scene to load. </param>
        /// <exception cref="ArgumentException"> Thrown when `id` is not a valid scene id (out of bounds) </exception>
        public void LoadScene(int id)
        {
            if (id < 0 || id >= RegisteredSceneCount)
                throw new ArgumentException($"Scene loader with id {id} does not exist! There are only {RegisteredSceneCount} scenes, you are out of bounds.");

            LoadScene(_sceneLoaders[id]);
        }

        // TODO: Change to a "load scene" function that picks the next scene right after it unloads the previous.
        public void UnloadSceneAtEndOfFrame()
        {
            _game.UpdateBegan.AddListener(LoadSceneAtEndOfFrame);
        }

        public void LoadScene(ISceneLoader loader)
        {
            // Do the loading
            _toLoadNext = loader;
            UnloadSceneAtEndOfFrame();
        }

        private void LoadSceneAtEndOfFrame()
        {
            // Queue for destroy and destroy all. This should work in tandem with this system.
            GameObjects.LoopThroughAll((obj) =>
            {
                obj.Destroy();
            });
            GameObjects.RemoveAllQueuedImmediate((obj) =>
            {
                obj.RunOnDestroy();
            });

            // Make sure we're only called once!
            _game.UpdateBegan.RemoveListener(LoadSceneAtEndOfFrame);

            _toLoadNext?.LoadScene();
            _toLoadNext = null;
        }
    }
}
