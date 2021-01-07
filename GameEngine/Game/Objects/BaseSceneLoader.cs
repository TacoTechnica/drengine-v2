using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameEngine.Game.Objects
{
    /// <summary>
    ///     A simple scene loader that should be the basis for any scene loader.
    /// </summary>
    public class BaseSceneLoader : ISceneLoader
    {
        protected GamePlus Game;
        private readonly string[] _names;
        private readonly Action<GamePlus> _onLoad;

        public BaseSceneLoader(GamePlus game, string[] names, Action<GamePlus> onLoad = null)
        {
            // If _game is null, we are dealing with serialization.
            if (game != null)
            {
                Game = game;
                _names = names;
                _onLoad = onLoad;

                UniqueId = game.SceneManager.RegisterSceneLoader(this);
            }
        }

        // One name constructor
        public BaseSceneLoader(GamePlus game, string name, Action<GamePlus> onLoad = null) : this(game, new[] {name},
            onLoad)
        {
        }

        [JsonIgnore] public int UniqueId { get; }

        public IEnumerable<string> GetNames()
        {
            return _names;
        }

        public virtual void LoadScene()
        {
            _onLoad?.Invoke(Game);
        }

        public void Deregister()
        {
            Game.SceneManager.DeregisterSceneLoader(this);
        }
    }
}