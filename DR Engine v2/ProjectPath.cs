using DREngine.Game;
using GameEngine.Game;

namespace DREngine
{
    public class ProjectPath : Path
    {
        private DRGame _game;
        public ProjectPath(DRGame game, string path) : base(path)
        {
            _game = game;
        }
        public override string ToString()
        {
            return _game.GameProjectData.GetFullProjectPath(_inputPath);
        }
    }
}
