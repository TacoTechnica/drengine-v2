using Microsoft.Xna.Framework;

namespace GameEngine.Util
{
    /// <summary>
    ///  A wrapper for System.Random.
    /// </summary>
    public class Random
    {
        private static int _seed = 0;
        private static System.Random _generator = new System.Random(_seed);

        public static int Seed
        {
            get => _seed;
            set
            {
                _seed = value;
                _generator = new System.Random(_seed);
            }
        }

        public static float Value => (float) _generator.NextDouble();

        public static float GetRange(float min, float max)
        {
            return min + Value * (max - min);
        }
        public static int GetRange(int min, int max)
        {
            return (int) (min + Value * (max - min));
        }

        public static Vector3 GetRange(Vector3 min, Vector3 max)
        {
            return new Vector3(
                    GetRange(min.X, max.X),
                    GetRange(min.Y, max.Y),
                    GetRange(min.Z, max.Z)
                );
        }

        public static Color Hue(float saturation, float value)
        {
            return Math.FromHSV(Value, saturation, value);
        }

        public static Color Color => new Color(GetRange(0, 255), GetRange(0, 255), GetRange(0, 255));
    }
}
