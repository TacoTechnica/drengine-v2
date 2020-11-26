
namespace DREngine
{
    /// <summary>
    /// cause I'm extra.
    /// </summary>
    public class Version
    {
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int UpdateNumber { get; private set; }

        public string Era { get; private set; }

        public Version(string era, int major, int minor, int updateNumber)
        {
            Era = era;
            Major = major;
            Minor = minor;
            UpdateNumber = updateNumber;
        }

        public static implicit operator string(Version v)
        {
            return v.ToString();
        }

        public override string ToString()
        {
            return $"{Era} {Major}.{Minor}.{UpdateNumber}";
        }
    }
}
