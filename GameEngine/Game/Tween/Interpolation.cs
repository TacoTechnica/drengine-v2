using System;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace GameEngine.Game.Tween
{
  /// <summary>
  ///     CREDITS TO JEFFREY LANTERS
  ///     https://github.com/elraccoone/unity-tweens
  ///     For making his awesome lib open source
  /// </summary>
  public static class Interpolation
    {
        private const float CONSTANT_A = 1.70158f;
        private const float CONSTANT_B = CONSTANT_A * 1.525f;
        private const float CONSTANT_C = CONSTANT_A + 1f;
        private const float CONSTANT_D = 2f * MathF.PI / 3f;
        private const float CONSTANT_E = 2f * MathF.PI / 4.5f;
        private const float CONSTANT_F = 7.5625f;
        private const float CONSTANT_G = 2.75f;

        public static float Apply(EaseType ease, float time)
        {
            switch (ease)
            {
                case EaseType.Linear:
                    return Linear(time);
                case EaseType.SineIn:
                    return SineIn(time);
                case EaseType.SineOut:
                    return SineOut(time);
                case EaseType.SineInOut:
                    return SineInOut(time);
                case EaseType.QuadIn:
                    return QuadIn(time);
                case EaseType.QuadOut:
                    return QuadOut(time);
                case EaseType.QuadInOut:
                    return QuadInOut(time);
                case EaseType.CubicIn:
                    return CubicIn(time);
                case EaseType.CubicOut:
                    return CubicOut(time);
                case EaseType.CubicInOut:
                    return CubicInOut(time);
                case EaseType.QuartIn:
                    return QuartIn(time);
                case EaseType.QuartOut:
                    return QuartOut(time);
                case EaseType.QuartInOut:
                    return QuartInOut(time);
                case EaseType.QuintIn:
                    return QuintIn(time);
                case EaseType.QuintOut:
                    return QuintOut(time);
                case EaseType.QuintInOut:
                    return QuintInOut(time);
                case EaseType.ExpoIn:
                    return ExpoIn(time);
                case EaseType.ExpoOut:
                    return ExpoOut(time);
                case EaseType.ExpoInOut:
                    return ExpoInOut(time);
                case EaseType.CircIn:
                    return CircIn(time);
                case EaseType.CircOut:
                    return CircOut(time);
                case EaseType.CircInOut:
                    return CircInOut(time);
                case EaseType.BackIn:
                    return BackIn(time);
                case EaseType.BackOut:
                    return BackOut(time);
                case EaseType.BackInOut:
                    return BackInOut(time);
                case EaseType.ElasticIn:
                    return ElasticIn(time);
                case EaseType.ElasticOut:
                    return ElasticOut(time);
                case EaseType.ElasticInOut:
                    return ElasticInOut(time);
                case EaseType.BounceIn:
                    return BounceIn(time);
                case EaseType.BounceOut:
                    return BounceOut(time);
                case EaseType.BounceInOut:
                    return BounceInOut(time);
                default:
                    throw new ArgumentOutOfRangeException(nameof(ease), ease, null);
            }
        }

        private static float Linear(float time)
        {
            return time;
        }

        private static float SineIn(float time)
        {
            return 1f - MathF.Cos(time * MathF.PI / 2f);
        }

        private static float SineOut(float time)
        {
            return MathF.Sin(time * MathF.PI / 2f);
        }

        private static float SineInOut(float time)
        {
            return -(MathF.Cos(MathF.PI * time) - 1f) / 2f;
        }

        private static float QuadIn(float time)
        {
            return time * time;
        }

        private static float QuadOut(float time)
        {
            return 1 - (1 - time) * (1 - time);
        }

        private static float QuadInOut(float time)
        {
            return time < 0.5f ? 2 * time * time : 1 - MathF.Pow(-2 * time + 2, 2) / 2;
        }

        private static float CubicIn(float time)
        {
            return time * time * time;
        }

        private static float CubicOut(float time)
        {
            return 1 - MathF.Pow(1 - time, 3);
        }

        private static float CubicInOut(float time)
        {
            return time < 0.5f ? 4 * time * time * time : 1 - MathF.Pow(-2 * time + 2, 3) / 2;
        }

        private static float QuartIn(float time)
        {
            return time * time * time * time;
        }

        private static float QuartOut(float time)
        {
            return 1 - MathF.Pow(1 - time, 4);
        }

        private static float QuartInOut(float time)
        {
            return time < 0.5 ? 8 * time * time * time * time : 1 - MathF.Pow(-2 * time + 2, 4) / 2;
        }

        private static float QuintIn(float time)
        {
            return time * time * time * time * time;
        }

        private static float QuintOut(float time)
        {
            return 1 - MathF.Pow(1 - time, 5);
        }

        private static float QuintInOut(float time)
        {
            return time < 0.5f ? 16 * time * time * time * time * time : 1 - MathF.Pow(-2 * time + 2, 5) / 2;
        }

        private static float ExpoIn(float time)
        {
            return time == 0 ? 0 : MathF.Pow(2, 10 * time - 10);
        }

        private static float ExpoOut(float time)
        {
            return time == 1 ? 1 : 1 - MathF.Pow(2, -10 * time);
        }

        private static float ExpoInOut(float time)
        {
            return time == 0 ? 0 :
                time == 1 ? 1 :
                time < 0.5 ? MathF.Pow(2, 20 * time - 10) / 2 : (2 - MathF.Pow(2, -20 * time + 10)) / 2;
        }

        private static float CircIn(float time)
        {
            return 1 - MathF.Sqrt(1 - MathF.Pow(time, 2));
        }

        private static float CircOut(float time)
        {
            return MathF.Sqrt(1 - MathF.Pow(time - 1, 2));
        }

        private static float CircInOut(float time)
        {
            return time < 0.5
                ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * time, 2))) / 2
                : (MathF.Sqrt(1 - MathF.Pow(-2 * time + 2, 2)) + 1) / 2;
        }

        private static float BackIn(float time)
        {
            return CONSTANT_C * time * time * time - CONSTANT_A * time * time;
        }

        private static float BackOut(float time)
        {
            return 1f + CONSTANT_C * MathF.Pow(time - 1, 3) + CONSTANT_A * MathF.Pow(time - 1, 2);
        }

        private static float BackInOut(float time)
        {
            return time < 0.5
                ? MathF.Pow(2 * time, 2) * ((CONSTANT_B + 1) * 2 * time - CONSTANT_B) / 2
                : (MathF.Pow(2 * time - 2, 2) * ((CONSTANT_B + 1) * (time * 2 - 2) + CONSTANT_B) + 2) / 2;
        }

        private static float ElasticIn(float time)
        {
            return time == 0 ? 0 :
                time == 1 ? 1 : -MathF.Pow(2, 10 * time - 10) * MathF.Sin((time * 10f - 10.75f) * CONSTANT_D);
        }

        private static float ElasticOut(float time)
        {
            return time == 0 ? 0 :
                time == 1 ? 1 : MathF.Pow(2, -10 * time) * MathF.Sin((time * 10 - 0.75f) * CONSTANT_D) + 1;
        }

        private static float ElasticInOut(float time)
        {
            return time == 0 ? 0 :
                time == 1 ? 1 :
                time < 0.5 ? -(MathF.Pow(2, 20 * time - 10) * MathF.Sin((20 * time - 11.125f) * CONSTANT_E)) / 2 :
                MathF.Pow(2, -20 * time + 10) * MathF.Sin((20 * time - 11.125f) * CONSTANT_E) / 2 + 1;
        }

        private static float BounceIn(float time)
        {
            return 1 - BounceOut(1 - time);
        }

        private static float BounceOut(float time)
        {
            if (time < 1 / CONSTANT_G)
                return CONSTANT_F * time * time;
            if (time < 2 / CONSTANT_G)
                return CONSTANT_F * (time -= 1.5f / CONSTANT_G) * time + 0.75f;
            if (time < 2.5f / CONSTANT_G)
                return CONSTANT_F * (time -= 2.25f / CONSTANT_G) * time + 0.9375f;
            return CONSTANT_F * (time -= 2.625f / CONSTANT_G) * time + 0.984375f;
        }

        private static float BounceInOut(float time)
        {
            return time < 0.5f ? (1 - BounceOut(1 - 2 * time)) / 2 : (1 + BounceOut(2 * time - 1)) / 2;
        }
    }

    public enum EaseType
    {
        Linear = 0,
        SineIn = 10,
        SineOut = 11,
        SineInOut = 12,
        QuadIn = 20,
        QuadOut = 21,
        QuadInOut = 22,
        CubicIn = 30,
        CubicOut = 31,
        CubicInOut = 32,
        QuartIn = 40,
        QuartOut = 41,
        QuartInOut = 42,
        QuintIn = 50,
        QuintOut = 51,
        QuintInOut = 52,
        ExpoIn = 60,
        ExpoOut = 61,
        ExpoInOut = 62,
        CircIn = 70,
        CircOut = 71,
        CircInOut = 72,
        BackIn = 80,
        BackOut = 81,
        BackInOut = 82,
        ElasticIn = 90,
        ElasticOut = 91,
        ElasticInOut = 92,
        BounceIn = 100,
        BounceOut = 101,
        BounceInOut = 102,
        Custom
    }
}