using System.Runtime.CompilerServices;

namespace Ryne.Utility.Math
{
    public class RyneMath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float v, float a, float b)
        {
            return System.Math.Max(a, System.Math.Min(v, b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 Mod(Float4 a, Float4 mod)
        {
            return new Float4(a.X % mod.X, a.Y % mod.Y, a.Z % mod.Z, a.W % mod.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 Mod(Float3 a, Float3 mod)
        {
            return new Float3(a.X % mod.X, a.Y % mod.Y, a.Z % mod.Z);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 Max(Float4 a, Float4 b)
        {
            return new Float4(System.Math.Max(a.X, b.X), System.Math.Max(a.Y, b.Y), System.Math.Max(a.Z, b.Z), System.Math.Max(a.W, b.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 Max(Float3 a, Float3 b)
        {
            return new Float3(System.Math.Max(a.X, b.X), System.Math.Max(a.Y, b.Y), System.Math.Max(a.Z, b.Z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 Min(Float4 a, Float4 b)
        {
            return new Float4(System.Math.Min(a.X, b.X), System.Math.Min(a.Y, b.Y), System.Math.Min(a.Z, b.Z), System.Math.Min(a.W, b.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 Min(Float3 a, Float3 b)
        {
            return new Float3(System.Math.Min(a.X, b.X), System.Math.Min(a.Y, b.Y), System.Math.Min(a.Z, b.Z));
        }


        // Overrides of default math to work with float instead of double

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float x)
        {
            return System.Math.Abs(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(float x)
        {
            return System.Math.Sign(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float x)
        {
            return (float)System.Math.Sin(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float x)
        {
            return (float)System.Math.Cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tan(float x)
        {
            return (float)System.Math.Tan(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan(float x)
        {
            return (float)System.Math.Atan(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float x, float y)
        {
            return (float)System.Math.Atan2(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Asin(float x)
        {
            return (float)System.Math.Asin(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float x)
        {
            return (float)System.Math.Acos(x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadiansToDegrees(float radians)
        {
            return radians / (float)System.Math.PI * 180.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegreesToRadians(float degrees)
        {
            return degrees * ((float)System.Math.PI / 180.0f);
        }

        // Returns the dominant axis as 0 (x), 1 (y) or 2 (z)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DominantAxis(Float3 value)
        {
            if (System.Math.Abs(value.X) > System.Math.Abs(value.Y) && System.Math.Abs(value.X) > System.Math.Abs(value.Z))
                return 0;

            if (System.Math.Abs(value.Y) > System.Math.Abs(value.Z))
                return 1;

            return 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float SampleAxis(Float3 value, int axis)
        {
            return (&value.X)[axis];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 Project(Float3 v1, Float3 v2)
        {
            return v1.Dot(v2 / v2.Length()) * v2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float4 Project(Float4 v1, Float4 v2)
        {
            return new Float4(Project(new Float3(v1), new Float3(v2)));
        }
    }
}
