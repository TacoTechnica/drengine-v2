using System;
using System.Collections.Generic;

namespace GameEngine.Game
{
    /// <summary>
    /// A simple scene loader that should be the basis for any scene loader.
    /// </summary>
    public class BaseSceneLoader  : ISceneLoader
    {
        private string[] _names;
        private GamePlus _game;
        private Action<GamePlus> _onLoad;

        public int UniqueId { get; private set; }
        public BaseSceneLoader(GamePlus game, string[] names, Action<GamePlus> onLoad)
        {
            _game = game;
            _names = names;
            _onLoad = onLoad;

            UniqueId = game.SceneManager.RegisterSceneLoader(this);
        }
        // One name constructor
        public BaseSceneLoader(GamePlus game, string name, Action<GamePlus> onLoad) : this(game, new string[]{name}, onLoad ){}
        public IEnumerable<string> GetNames()
        {
            return _names;
        }

        public void LoadScene()
        {
            _onLoad.Invoke(_game);
        }
    }
}
