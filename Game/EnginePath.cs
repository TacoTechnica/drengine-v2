
namespace DREngine.Game {
    public class EnginePath : Path {

        public EnginePath(string path) : base($"{Program.RootDirectory}/{path}") {}
    }
}
