using System.Collections.Generic;

namespace DREngine.Game
{
    /// <summary>
    /// TODO: Implement this.
    ///     - When adding an object to the game, the game adds it to the current scene.
    ///     - The scene manager is responsible for loading/unloading scenes that you pass to it. That's it.
    ///     - When you load a scene, it takes the old scene (if there was any) and deletes all of the objects.
    ///     - A scene can have objects requested to be added to it and removed from it at any time
    /// </summary>
    public class SceneManager
    {

        private GamePlus _game;
        internal ObjectContainer<GameObject> GameObjects { get; private set; }
        internal ObjectContainer<GameObjectRender> GameRenderObjects { get; private set; }
        internal ObjectContainer<Camera3D> Cameras { get; private set; }

        public SceneManager(GamePlus game)
        {
            _game = game;
            // Tracked objects
            GameObjects = new ObjectContainer<GameObject>();
            GameRenderObjects = new ObjectContainer<GameObjectRender>();
            Cameras = new ObjectContainer<Camera3D>();
        }

        // TODO: Change to a "load scene" function that picks the next scene right after it unloads the previous.
        public void UnloadScene()
        {
            _game.UpdateFinished.AddListener(UnloadSceneAtEndOfFrame);
        }

        private void UnloadSceneAtEndOfFrame()
        {
            // Queue for destroy and destroy all. This should work in tandem with this system.
            GameObjects.LoopThroughAll((obj) =>
            {
                obj.Destroy();
            });
            GameObjects.RemoveAllQueuedImmediate((gobj) =>
            {
                gobj.RunOnDestroy();
            });

            // Make sure we're only called once!
            _game.UpdateFinished.RemoveListener(UnloadSceneAtEndOfFrame);
        }
    }
}
