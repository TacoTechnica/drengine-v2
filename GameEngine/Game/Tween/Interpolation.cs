using System;


namespace GameEngine.Game.Tween
{

    /// <summary>
    /// CREDITS TO JEFFREY LANTERS
    /// https://github.com/elraccoone/unity-tweens
    /// For making his awesome lib open source
    /// </summary>
    public static class Interpolation
    {
        private const float constantA = 1.70158f;
    private const float constantB = constantA * 1.525f;
    private const float constantC = constantA + 1f;
    private const float constantD = (2f * MathF.PI) / 3f;
    private const float constantE = (2f * MathF.PI) / 4.5f;
    private const float constantF = 7.5625f;
    private const float constantG = 2.75f;

    public static float Apply (EaseType ease, float time) {
      switch (ease) {
        case EaseType.Linear:
          return Interpolation.Linear (time);
        case EaseType.SineIn:
          return Interpolation.SineIn (time);
        case EaseType.SineOut:
          return Interpolation.SineOut (time);
        case EaseType.SineInOut:
          return Interpolation.SineInOut (time);
        case EaseType.QuadIn:
          return Interpolation.QuadIn (time);
        case EaseType.QuadOut:
          return Interpolation.QuadOut (time);
        case EaseType.QuadInOut:
          return Interpolation.QuadInOut (time);
        case EaseType.CubicIn:
          return Interpolation.CubicIn (time);
        case EaseType.CubicOut:
          return Interpolation.CubicOut (time);
        case EaseType.CubicInOut:
          return Interpolation.CubicInOut (time);
        case EaseType.QuartIn:
          return Interpolation.QuartIn (time);
        case EaseType.QuartOut:
          return Interpolation.QuartOut (time);
        case EaseType.QuartInOut:
          return Interpolation.QuartInOut (time);
        case EaseType.QuintIn:
          return Interpolation.QuintIn (time);
        case EaseType.QuintOut:
          return Interpolation.QuintOut (time);
        case EaseType.QuintInOut:
          return Interpolation.QuintInOut (time);
        case EaseType.ExpoIn:
          return Interpolation.ExpoIn (time);
        case EaseType.ExpoOut:
          return Interpolation.ExpoOut (time);
        case EaseType.ExpoInOut:
          return Interpolation.ExpoInOut (time);
        case EaseType.CircIn:
          return Interpolation.CircIn (time);
        case EaseType.CircOut:
          return Interpolation.CircOut (time);
        case EaseType.CircInOut:
          return Interpolation.CircInOut (time);
        case EaseType.BackIn:
          return Interpolation.BackIn (time);
        case EaseType.BackOut:
          return Interpolation.BackOut (time);
        case EaseType.BackInOut:
          return Interpolation.BackInOut (time);
        case EaseType.ElasticIn:
          return Interpolation.ElasticIn (time);
        case EaseType.ElasticOut:
          return Interpolation.ElasticOut (time);
        case EaseType.ElasticInOut:
          return Interpolation.ElasticInOut (time);
        case EaseType.BounceIn:
          return Interpolation.BounceIn (time);
        case EaseType.BounceOut:
          return Interpolation.BounceOut (time);
        case EaseType.BounceInOut:
          return Interpolation.BounceInOut (time);
        default:
          throw new ArgumentOutOfRangeException(nameof(ease), ease, null);
      }
    }

    private static float Linear (float time) {
      return time;
    }

    private static float SineIn (float time) {
      return 1f - MathF.Cos ((time * MathF.PI) / 2f);
    }

    private static float SineOut (float time) {
      return MathF.Sin ((time * MathF.PI) / 2f);
    }

    private static float SineInOut (float time) {
      return -(MathF.Cos (MathF.PI * time) - 1f) / 2f;
    }

    private static float QuadIn (float time) {
      return time * time;
    }

    private static float QuadOut (float time) {
      return 1 - (1 - time) * (1 - time);
    }

    private static float QuadInOut (float time) {
      return time < 0.5f ? 2 * time * time : 1 - MathF.Pow (-2 * time + 2, 2) / 2;
    }

    private static float CubicIn (float time) {
      return time * time * time;
    }

    private static float CubicOut (float time) {
      return 1 - MathF.Pow (1 - time, 3);
    }

    private static float CubicInOut (float time) {
      return time < 0.5f ? 4 * time * time * time : 1 - MathF.Pow (-2 * time + 2, 3) / 2;
    }

    private static float QuartIn (float time) {
      return time * time * time * time;
    }

    private static float QuartOut (float time) {
      return 1 - MathF.Pow (1 - time, 4);
    }

    private static float QuartInOut (float time) {
      return time < 0.5 ? 8 * time * time * time * time : 1 - MathF.Pow (-2 * time + 2, 4) / 2;
    }

    private static float QuintIn (float time) {
      return time * time * time * time * time;
    }

    private static float QuintOut (float time) {
      return 1 - MathF.Pow (1 - time, 5);
    }

    private static float QuintInOut (float time) {
      return time < 0.5f ? 16 * time * time * time * time * time : 1 - MathF.Pow (-2 * time + 2, 5) / 2;
    }

    private static float ExpoIn (float time) {
      return time == 0 ? 0 : MathF.Pow (2, 10 * time - 10);
    }

    private static float ExpoOut (float time) {
      return time == 1 ? 1 : 1 - MathF.Pow (2, -10 * time);
    }

    private static float ExpoInOut (float time) {
      return time == 0 ? 0 : time == 1 ? 1 : time < 0.5 ? MathF.Pow (2, 20 * time - 10) / 2 : (2 - MathF.Pow (2, -20 * time + 10)) / 2;
    }

    private static float CircIn (float time) {
      return 1 - MathF.Sqrt (1 - MathF.Pow (time, 2));
    }

    private static float CircOut (float time) {
      return MathF.Sqrt (1 - MathF.Pow (time - 1, 2));
    }

    private static float CircInOut (float time) {
      return time < 0.5 ? (1 - MathF.Sqrt (1 - MathF.Pow (2 * time, 2))) / 2 : (MathF.Sqrt (1 - MathF.Pow (-2 * time + 2, 2)) + 1) / 2;
    }

    private static float BackIn (float time) {
      return constantC * time * time * time - constantA * time * time;
    }

    private static float BackOut (float time) {
      return 1f + constantC * MathF.Pow (time - 1, 3) + constantA * MathF.Pow (time - 1, 2);
    }

    private static float BackInOut (float time) {
      return time < 0.5 ?
        (MathF.Pow (2 * time, 2) * ((constantB + 1) * 2 * time - constantB)) / 2 :
        (MathF.Pow (2 * time - 2, 2) * ((constantB + 1) * (time * 2 - 2) + constantB) + 2) / 2;
    }

    private static float ElasticIn (float time) {
      return time == 0 ? 0 : time == 1 ? 1 : -MathF.Pow (2, 10 * time - 10) * MathF.Sin ((time * 10f - 10.75f) * constantD);
    }

    private static float ElasticOut (float time) {
      return time == 0 ? 0 : time == 1 ? 1 : MathF.Pow (2, -10 * time) * MathF.Sin ((time * 10 - 0.75f) * constantD) + 1;
    }

    private static float ElasticInOut (float time) {
      return time == 0 ? 0 : time == 1 ? 1 : time < 0.5 ? -(MathF.Pow (2, 20 * time - 10) * MathF.Sin ((20 * time - 11.125f) * constantE)) / 2 : (MathF.Pow (2, -20 * time + 10) * MathF.Sin ((20 * time - 11.125f) * constantE)) / 2 + 1;
    }

    private static float BounceIn (float time) {
      return 1 - Interpolation.BounceOut (1 - time);
    }

    private static float BounceOut (float time) {
      if (time < 1 / constantG)
        return constantF * time * time;
      else if (time < 2 / constantG)
        return constantF * (time -= 1.5f / constantG) * time + 0.75f;
      else if (time < 2.5f / constantG)
        return constantF * (time -= 2.25f / constantG) * time + 0.9375f;
      else
        return constantF * (time -= 2.625f / constantG) * time + 0.984375f;
    }

    private static float BounceInOut (float time) {
      return time < 0.5f ? (1 - Interpolation.BounceOut (1 - 2 * time)) / 2 : (1 + Interpolation.BounceOut (2 * time - 1)) / 2;
    }
  }
    public enum EaseType {
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
