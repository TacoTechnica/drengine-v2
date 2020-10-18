
using System;
using Microsoft.Xna.Framework;

/// <summary>
///     Math stuff that really should be included in every game engine.
/// </summary>

namespace DREngine
{
    public static class Math
    {

        /* I give up, -z must be forward from now on.
        /// Because Monogame's forward is -1 z. I know.
        private static readonly Vector3 forward = new Vector3(1, 0, 0);
        private static readonly Vector3 backward = new Vector3(-1, 0, 0);
        private static readonly Vector3 up = new Vector3(0, -1, 0);
        private static readonly Vector3 down = new Vector3(0, 1, 0);
        private static readonly Vector3 left = new Vector3(0, 0, -1);
        private static readonly Vector3 right = new Vector3(0, 0, 1);
        public static Vector3 Forward => forward;
        public static Vector3 Backward => backward;
        public static Vector3 Up => up;
        public static Vector3 Down => down;
        public static Vector3 Left => left;
        public static Vector3 Right => right;
        */

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
    }
}
