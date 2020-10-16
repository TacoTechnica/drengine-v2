
using System;

/// <summary>
///     Math stuff that really should be included in every game engine.
/// </summary>

namespace DREngine
{
    public static class Math
    {


        public static float Mod(float x, float m)
        {
            if (x < 0)
            {
                return m - (x % m);
            }
            return x % m;
        }

        public static float Abs(float x)
        {
            return MathF.Abs(x);
        }

        public static int Sign(float x)
        {
            return MathF.Sign(x);
        }

        public static float AngleDifference(float to, float from, float halfCircle=180f)
        {
            float delta = Mod(to - from, halfCircle*2);
            if (Abs(delta) > halfCircle)
            {
                return Sign(delta) * (halfCircle - delta);
            }

            return delta;
        }
    }
}
