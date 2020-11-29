using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameEngine.Game
{
    /// <summary>
    /// A simple scene loader that should be the basis for any scene loader.
    /// </summary>
    public class BaseSceneLoader  : ISceneLoader
    {
        private string[] _names;
        protected GamePlus _game;
        private Action<GamePlus> _onLoad;

        [JsonIgnore]
        public int UniqueId { get; private set; }
        public BaseSceneLoader(GamePlus game, string[] names, Action<GamePlus> onLoad = null)
        {
            // If _game is null, we are dealing with serialization.
            if (game != null)
            {
                _game = game;
                _names = names;
                _onLoad = onLoad;

                UniqueId = game.SceneManager.RegisterSceneLoader(this);
            }
        }

        public void Deregister()
        {
            _game.SceneManager.DeregisterSceneLoader(this);
        }

        // One name constructor
        public BaseSceneLoader(GamePlus game, string name, Action<GamePlus> onLoad = null) : this(game, new string[]{name}, onLoad ){}
        public IEnumerable<string> GetNames()
        {
            return _names;
        }

        public virtual void LoadScene()
        {
            _onLoad?.Invoke(_game);
        }
    }
}
