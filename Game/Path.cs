namespace DREngine.Game
{
    /// <summary>
    /// Represents a path and is to be used for all
    /// resource files requiring a path.
    ///
    /// Q: Why a wrapper for a string?
    /// A: Say we want to use different kinds of paths.
    ///         Project relative paths, Game relative paths, etc.
    ///         If we use a string we will have to convert every time.
    ///         But if we implement this class and override ToString,
    ///         it will work cleanly.
    /// </summary>
    public class Path
    {
        protected string _inputPath;
        public Path(string path)
        {
            _inputPath = path.Replace('\\', '/');
        }

        // Makes it so that we can use gamepaths instead of strings. Very handy.
        public static implicit operator string(Path p)
        {
            return p.ToString();
        }

        public override string ToString()
        {
            return _inputPath;
        }
    }
}
