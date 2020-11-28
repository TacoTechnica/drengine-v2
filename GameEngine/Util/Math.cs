
using System;
using Microsoft.Xna.Framework;

namespace GameEngine
{

    /// <summary>
    ///     Math stuff that really should be included in every game engine.
    /// </summary>
    public static class Math
    {

        public static float Deg2Rad = MathF.PI / 180F;
        public static float Rad2Deg = 180F / MathF.PI;

        public static float Mod(float x, float m)
        {
            if (x < 0)
            {
                return m - (Abs(x) % m);
            }
            return x % m;
        }
        public static int Mod(int x, int m)
        {
            if (x < 0)
            {
                return (m - (int)(Abs(x) % m)) % m;
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

        public static float CopySign(float x, float s)
        {
            return Abs(x) * Sign(s);
        }
        public static float CopySign(int x, int s)
        {
            return Abs(x) * Sign(s);
        }

        public static float Threshold(float x, float threshold)
        {
            if (Abs(x) < threshold) return 0;
            return x;
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

        public static float NormalizeAngle(float angle, float circle = 360F)
        {
            return Mod(angle, circle);
        }

        public static Vector3 NormalizeAngle(Vector3 angles, float circle = 360F)
        {
            angles.X = NormalizeAngle(angles.X, circle);
            angles.Y = NormalizeAngle(angles.Y, circle);
            angles.Z = NormalizeAngle(angles.Z, circle);
            return angles;
        }

        public static float AddAngleAndClamp(float angle, float delta, float min, float max)
        {
            float dMin = AngleDifference(angle, min),
                dMax = AngleDifference(angle, max);
            bool aboveMin = dMin > 0;
            bool belowMax = dMax < 0;

            bool failedBefore = !(aboveMin && belowMax);

            // We're inside of range. Add and check.
            angle += delta;

            dMin = AngleDifference(angle, min);
            dMax = AngleDifference(angle, max);
            aboveMin = dMin > 0;
            belowMax = dMax < 0;

            if (!(aboveMin && belowMax))
            {
                if (failedBefore)
                {
                    // We're outside of range already, expect snappy behaviour.
                    return (Abs(dMin) < Abs(dMax))? min : max;
                }

                // We WERE inside and now have moved outside. This check is easier.
                return (delta < 0) ? min : max;
            }

            return angle;
        }

        public static Quaternion FromEuler (Vector3 v)
        {
            return FromEuler (v.X, v.Y, v.Z);
        }

        public static Quaternion FromEuler (float pitch, float yaw, float roll)
        {
            yaw *= Math.Deg2Rad;
            pitch *= Math.Deg2Rad;
            roll *= Math.Deg2Rad;
            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = MathF.Sin (rollOver2);
            float cosRollOver2 = MathF.Cos (rollOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = MathF.Sin (pitchOver2);
            float cosPitchOver2 = MathF.Cos (pitchOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = MathF.Sin (yawOver2);
            float cosYawOver2 = MathF.Cos (yawOver2);
            Quaternion result;
            result.W = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.X = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.Y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            result.Z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

            return result;
        }

        public static Vector3 ToEuler (Quaternion q1)
        {
            float sqw = q1.W * q1.W;
            float sqx = q1.X * q1.X;
            float sqy = q1.Y * q1.Y;
            float sqz = q1.Z * q1.Z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = q1.X * q1.W - q1.Y * q1.Z;
            Vector3 v;

            if (test>0.4995f*unit) { // singularity at north pole
                v.Y = 2f * MathF.Atan2 (q1.Y, q1.X);
                v.X = MathF.PI / 2;
                v.Z = 0;
                return NormalizeAngle (v * Rad2Deg);
            }
            if (test<-0.4995f*unit) { // singularity at south pole
                v.Y = -2f * MathF.Atan2 (q1.Y, q1.X);
                v.X = -MathF.PI / 2;
                v.Z = 0;
                return NormalizeAngle (v * Rad2Deg);
            }
            Quaternion q = new Quaternion (q1.W, q1.Z, q1.X, q1.Y);
            v.Y = (float)MathF.Atan2 (2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W));    // Yaw
            v.X = (float)MathF.Asin (2f * (q.X * q.Z - q.W * q.Y));                                             // Pitch
            v.Z = (float)MathF.Atan2 (2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z));    // Roll
            return NormalizeAngle (v * Rad2Deg);
        }


        /// <summary>
        ///     All hail stack exchange https://gamedev.stackexchange.com/questions/28395/rotating-vector3-by-a-quaternion
        /// </summary>
        /// <param name="vec"> An input vector. </param>
        /// <param name="rotation"> A quaternion rotation. </param>
        /// <returns> `vec` rotated by `rotation`. </returns>
        public static Vector3 RotateVector(Vector3 vec, Quaternion rotation)
        {
            Vector3 u = new Vector3(rotation.X, rotation.Y, rotation.Z);

            // Extract the scalar part of the quaternion
            float s = rotation.W;

            // Do the math
            return 2.0f * Vector3.Dot(u, vec) * u
                     + (s*s - Vector3.Dot(u, u)) * vec
                     + 2.0f * s * Vector3.Cross(u, vec);
        }

        public static Vector2 ClampMagnitude(Vector2 vec, float MaxMag)
        {
            if (vec.LengthSquared() > MaxMag * MaxMag)
            {
                vec.Normalize();
                return vec * MaxMag;
            }
            return vec;
        }

        public static Vector3 ClampMagnitude(Vector3 vec, float MaxMag)
        {
            if (vec.LengthSquared() > MaxMag * MaxMag)
            {
                vec.Normalize();
                return vec * MaxMag;
            }
            return vec;
        }

        public static float Sqrt(float value)
        {
            return MathF.Sqrt(value);
        }

        public static float Clamp01(float value)
        {
            return Clamp(value, 0, 1);
        }

        public static float Clamp(float value, float min, float max)
        {
            return System.Math.Clamp(value, min, max);
        }
        public static int Clamp(int value, int min, int max)
        {
            return System.Math.Clamp(value, min, max);
        }

        public static int Min(int a, int b)
        {
            return System.Math.Min(a, b);
        }
        public static int Max(int a, int b)
        {
            return System.Math.Max(a, b);
        }
        public static float Min(float a, float b)
        {
            return System.Math.Min(a, b);
        }

        public static float Max(float a, float b)
        {
            return System.Math.Max(a, b);
        }

        public static float Floor(float x)
        {
            return MathF.Floor(x);
        }
        public static float Ceil(float x)
        {
            return MathF.Ceiling(x);
        }

        public static int FloorToInt(float x)
        {
            return (int) Floor(x);
        }
        public static int CeilToInt(float x)
        {
            return (int) Ceil(x);
        }

        public static Color FromHSV(float h, float s, float v)
        {
            // Convert from 0-360 to 0-1.
            h *= 360f;
            int hi = Convert.ToInt32(System.Math.Floor(h / 60)) % 6;
            double f = h / 60 - System.Math.Floor(h / 60);

            v = v * 255;
            int x = Convert.ToInt32(v);
            int p = Convert.ToInt32(v * (1 - s));
            int q = Convert.ToInt32(v * (1 - f * s));
            int t = Convert.ToInt32(v * (1 - (1 - f) * s));

            if (hi == 0)
                return new Color(x, t, p);
            else if (hi == 1)
                return new Color(q, x, p);
            else if (hi == 2)
                return new Color(p, x, t);
            else if (hi == 3)
                return new Color(p, q, x);
            else if (hi == 4)
                return new Color(t, p, x);
            else
                return new Color(x, p, q);
        }
    }
}
