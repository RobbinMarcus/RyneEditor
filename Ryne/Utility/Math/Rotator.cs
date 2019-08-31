using System.Diagnostics;

namespace Ryne.Utility.Math
{
    [DebuggerDisplay("X = {X} Y = {Y} Z = {Z}")]
    public struct Rotator
    {
        // [Yaw, Pitch, Roll]
        public float X, Y, Z;

        public Rotator(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Rotator(Float3 data)
        {
            SetData(out this, data);
        }

        public Rotator(Float4 data)
        {
            SetData(out this, new Float3(data));
        }

        public Quaternion ToQuaternion()
        {
            // https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
            // [X=Yaw, Y=Pitch, Z=Roll] to Quaternion
            float yaw = RyneMath.DegreesToRadians(X) * 0.5f;
            float pitch = RyneMath.DegreesToRadians(Y) * 0.5f;
            float roll = RyneMath.DegreesToRadians(Z) * 0.5f;

            float cy = RyneMath.Cos(yaw);
            float sy = RyneMath.Sin(yaw);
            float cp = RyneMath.Cos(pitch);
            float sp = RyneMath.Sin(pitch);
            float cr = RyneMath.Cos(roll);
            float sr = RyneMath.Sin(roll);

            Quaternion result = new Quaternion
            {
                W = cy * cp * cr + sy * sp * sr,
                X = cy * cp * sr - sy * sp * cr,
                Y = sy * cp * sr + cy * sp * cr,
                Z = sy * cp * cr - cy * sp * sr
            };
            return result;
        }
        private static void SetData(out Rotator r, Float3 data)
        {
            r.X = data.X;
            r.Y = data.Y;
            r.Z = data.Z;
        }

        public Float3 ToFloat3()
        {
            return new Float3(X, Y, Z);
        }

        public Float4 ToFloat4()
        {
            return new Float4(X, Y, Z, 0.0f);
        }

        public override string ToString()
        {
            return "X=" + X + " Y=" + Y + " Z=" + Z;
        }
    }
}
