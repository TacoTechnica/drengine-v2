using System.Diagnostics;
using System.Reflection;

namespace GameEngine.Game
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
        public string RelativePath;
        public Path(string path)
        {
            RelativePath = path.Replace('\\', '/');
        }

        // Makes it so that we can use gamepaths instead of strings. Very handy.
        public static implicit operator string(Path p)
        {
            return p?.ToString();
        }

        public static implicit operator Path(string s)
        {
            return new Path(s);
        }

        public override string ToString()
        {
            return RelativePath;
        }

        public virtual string GetShortName()
        {
            return ToString();
        }

        protected virtual Path CreateNew(string relativePath)
        {
            return new Path(relativePath);
        }

        public static Path operator +(Path a, string b)
            => a.CreateNew(a.RelativePath + b);
    }

    public class EnginePath : Path {

        public EnginePath(string path) : base(path) {}

        public override string ToString()
        {
            return $"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}/{RelativePath}";
        }

        public override string GetShortName()
        {
            return "ENGINEPATH://" + RelativePath;
        }
        
        protected override Path CreateNew(string relativePath)
        {
            return new EnginePath(relativePath);
        }
        
    }
}
