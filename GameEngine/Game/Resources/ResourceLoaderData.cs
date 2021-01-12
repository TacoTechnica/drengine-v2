using System;
using GameEngine.Game.Audio;
using GameEngine.Util;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Game.Resources
{
    /// <summary>
    ///     Contains EVERYTHING we need to load resources.
    ///     Exists to create a BARRIER between GAME and RESOURCE LOADING (so Editor can load resources too)
    /// </summary>
    public class ResourceLoaderData
    {
        public AudioOutput AudioOutput;
        public GraphicsDevice GraphicsDevice;

        private bool _safeToLoad = false;

        private readonly EventManager
            _whenSafeToLoad = new EventManager(true);

        public void Initialize(GraphicsDevice graphicsDevice, AudioOutput audioOutput)
        {
            AudioOutput = audioOutput;
            GraphicsDevice = graphicsDevice;
            _safeToLoad = true;
            _whenSafeToLoad.InvokeAll();
        }

        public void LoadWhenSafe(Action onSafeToLoad)
        {
            if (!_safeToLoad)
                _whenSafeToLoad.AddListener(onSafeToLoad);
            else
                onSafeToLoad.Invoke();
        }
    }
}