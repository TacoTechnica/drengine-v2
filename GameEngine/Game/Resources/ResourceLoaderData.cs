using GameEngine.Game.Audio;
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

        public void Initialize(GraphicsDevice graphicsDevice, AudioOutput audioOutput)
        {
            AudioOutput = audioOutput;
            GraphicsDevice = graphicsDevice;
        }
    }
}