using System.Collections.Generic;

namespace GameEngine.Game
{
    /// <summary>
    /// Represents a scene that can load objects. That's pretty much it.
    /// </summary>
    public interface ISceneLoader
    {
        /// <summary>
        /// This is used for string access of a scene.
        /// </summary>
        /// <returns>  </returns>
        IEnumerable<string> GetNames();
        void LoadScene();
    }
}
