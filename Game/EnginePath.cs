
namespace DREngine.Game {
    public class EnginePath : GamePath {

        public EnginePath(string path) : base($"{Program.RootDirectory}/{path}") {}
    }
}
