using DREngine.Game;
using GameEngine.Game;

namespace DREngine
{
    public class ProjectPath : Path
    {
        private DRGame _game;

        private string _fullProjectPath;
        public ProjectPath(DRGame game, string path) : base(path)
        {
            _game = game;
            _fullProjectPath = null;
        }

        public ProjectPath(string fullProjectPath, string path) : base(path)
        {
            _game = null;
            _fullProjectPath = fullProjectPath;
        }
        public override string ToString()
        {
            return _game != null? _game.GameProjectData.GetFullProjectPath(RelativePath) : ProjectData.GetFullProjectPath(_fullProjectPath, RelativePath);
        }
    }
}
