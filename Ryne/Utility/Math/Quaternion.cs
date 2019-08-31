using System;
using System.Diagnostics;

namespace Ryne.Utility.Math
{
    [DebuggerDisplay("X={X} Y={Y} Z={Z} W={W}")]
    public struct Quaternion
    {
        // [Roll, Pitch, Yaw]
        public float X, Y, Z, W;

        public static readonly Quaternion Default = new Quaternion(0, 0, 0, 1);

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Quaternion(Float4 data)
        {
            SetData(out this, data);
        }

        public Quaternion(Float3 axis, float angle, bool degrees = true)
        {
            //qx = ax * sin(angle/2)
            //qy = ay * sin(angle/2)
            //qz = az * sin(angle/2)
            //qw = cos(angle/2)

            if (degrees)
            {
                angle = RyneMath.DegreesToRadians(angle);
            }

            float halfAngle = angle * 0.5f;
            Float4 data = new Float4(axis * RyneMath.Sin(halfAngle), RyneMath.Cos(halfAngle)).Normalize();
            SetData(out this, data);
        }

        public Quaternion Multiply(Quaternion other)
        {
            Float3 vA = ToFloat3();
            Float3 vB = other.ToFloat3();

            Float4 result = new Float4(vA.Cross(vB) + vA * other.W + vB * W, W * other.W - vA.Dot(vB));
            return new Quaternion(result);
        }

        public void Invert()
        {
            SetData(out this, new Float4(-X, -Y, -Z, W));
        }

        // Returns the conjugate which is equal to the inverse for unit quaternions
        public Quaternion Inverse()
        {
            return new Quaternion(new Float4(-X, -Y, -Z, W));
        }

        // TODO: untested
        public Quaternion Interpolate(Quaternion to, float dt)
        {
            Float4 v0 = ToFloat4();
            Float4 v1 = to.ToFloat4();
            float dot = v0.Dot(v1);

            if (dot < 0.0f)
            {
                v1 = new Float4(-v1.X, -v1.Y, -v1.Z, -v1.W);
                dot = -dot;
            }

            if (dot > 0.9995f)
            {
                return to;
                //// If the inputs are too close for comfort, linearly interpolate and normalize the result
                //Float4 result = v0 + dt * (v1 - v0);
                //result.Normalize();
                //return new Quaternion(result);
            }

            float theta = RyneMath.Acos(dot);
            float deltaTheta = theta * dt;
            float sinTheta = RyneMath.Sin(theta);
            float sinDeltaTheta = RyneMath.Sin(deltaTheta);

            float s0 = RyneMath.Cos(deltaTheta) - dot * sinDeltaTheta / sinTheta;
            float s1 = sinDeltaTheta / sinTheta;

            return new Quaternion((s0 * v0) + (s1 * v1));
        }

        public Float4 RotateVector(Float4 vector)
        {
            Float3 u = ToFloat3();
            Float3 v = new Float3(vector);
            Float3 result = 2.0f * u.Dot(v) * u;
            result += (W * W - u.Dot(u)) * v;
            result += 2.0f * W * u.Cross(v);
            return new Float4(result);
        }

        public Float4 RotateVectorAroundPivot(Float4 vector, Float4 pivot)
        {
            return RotateVector(vector - pivot) + pivot;
        }

        public Float4 GetForwardVector()
        {
            return RotateVector(new Float4(1, 0, 0, 0));
        }

        public Float4 GetRightVector()
        {
            return RotateVector(new Float4(0, 1, 0, 0));
        }

        public Float4 GetUpVector()
        {
            return RotateVector(new Float4(0, 0, -1, 0));
        }

        public Rotator ToRotator()
        {
            // http://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
            Rotator result = new Rotator();

            // Roll (rotation around x-axis)
            float sinr = 2.0f * (W * X + Y * Z);
            float cosr = 1.0f - 2.0f * (X * X + Y * Y);
            result.Z = RyneMath.Atan2(sinr, cosr);

            // Pitch (rotation around y-axis)
            float sinp = 2.0f * (W * Y - Z * X);
            if (RyneMath.Abs(sinp) >= 1)
            {
                result.Y = Constants.PI / 2.0f * RyneMath.Sign(sinp); // use 90 degrees if out of range
            }
            else
            {
                result.Y = RyneMath.Asin(sinp);
            }

            // Yaw (rotation around z-axis)
            float siny = 2.0f * (W * Z + X * Y);
            float cosy = 1.0f - 2.0f * (Y * Y + Z * Z);
            result.X = RyneMath.Atan2(siny, cosy);

            result.X = RyneMath.RadiansToDegrees(result.X);
            result.Y = RyneMath.RadiansToDegrees(result.Y);
            result.Z = RyneMath.RadiansToDegrees(result.Z);
            return result;
        }

        private static void SetData(out Quaternion q, Float4 data)
        {
            q.X = data.X;
            q.Y = data.Y;
            q.Z = data.Z;
            q.W = data.W;
        }

        public Float3 ToFloat3()
        {
            return new Float3(X, Y, Z);
        }

        public Float4 ToFloat4()
        {
            return new Float4(X, Y, Z, W);
        }

        public override string ToString()
        {
            return "X=" + X + " Y=" + Y + " Z=" + Z + " W=" + W;
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return a.Multiply(b);
        }

        public float RotationAngle(bool degrees = true)
        {
            return RyneMath.RadiansToDegrees(2.0f * RyneMath.Acos(W));
        }
    }
}
