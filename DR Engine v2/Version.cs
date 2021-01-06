namespace DREngine
{
    /// <summary>
    ///     cause I'm extra.
    /// </summary>
    public class Version
    {
        public Version(string era, int major, int minor, int updateNumber)
        {
            Era = era;
            Major = major;
            Minor = minor;
            UpdateNumber = updateNumber;
        }

        public int Major { get; }
        public int Minor { get; }
        public int UpdateNumber { get; }

        public string Era { get; }

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