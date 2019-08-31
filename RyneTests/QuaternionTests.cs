using Ryne;
using Ryne.Utility.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class QuaternionTests
    {
        [TestMethod]
        public void EmptyQuaternionRotateVector()
        {
            Float4 vector = new Float4(10, 0, 0, 0);
            Quaternion quaternion = Quaternion.Default;

            Float4 result = quaternion.RotateVector(vector);
            Assert.AreEqual(new Float4(10, 0, 0, 0), result);
        }

        [TestMethod]
        public void EmptyRotatorToQuaternion()
        {
            Rotator rotator = new Rotator();
            Quaternion quaternion = rotator.ToQuaternion();
            Assert.AreEqual(new Float4(0, 0, 0, 1), quaternion.ToFloat4());
        }

        [TestMethod]
        public void EmptyQuaternionToRotator()
        {
            Quaternion quaternion = Quaternion.Default;
            Rotator rotator = quaternion.ToRotator();
            Assert.AreEqual(new Float3(0, 0, 0), rotator.ToFloat3());
        }

        [TestMethod]
        public void EmptyQuaternionMultiplyEmptyQuaternion()
        {
            Quaternion quaternion1 = Quaternion.Default;
            Quaternion quaternion2 = Quaternion.Default;

            Quaternion result = quaternion1.Multiply(quaternion2);
            Assert.AreEqual(new Float4(0, 0, 0, 1), result.ToFloat4());
        }




        [TestMethod]
        public void QuaternionRotateVector90DegreesPosY()
        {
            Float4 vector = new Float4(10, 0, 0, 0);
            Quaternion quaternion = new Quaternion(new Float4(0, 0, 1, 1).Normalize());

            Float4 result = quaternion.RotateVector(vector);
            Assert.AreEqual(new Float4(0, 10, 0, 0), result);
        }

        [TestMethod]
        public void QuaternionRotateVector90DegreesNegY()
        {
            Float4 vector = new Float4(10, 0, 0, 0);
            Quaternion quaternion = new Quaternion(new Float4(0, 0, -1, 1).Normalize());

            Float4 result = quaternion.RotateVector(vector);
            Assert.AreEqual(new Float4(0, -10, 0, 0), result);
        }

        [TestMethod]
        public void QuaternionRotateVector90DegreesPosZ()
        {
            Float4 vector = new Float4(10, 0, 0, 0);
            Quaternion quaternion = new Quaternion(new Float4(0, 1, 0, 1).Normalize());

            Float4 result = quaternion.RotateVector(vector);
            Assert.AreEqual(new Float4(0, 0, -10, 0), result);
        }

        [TestMethod]
        public void QuaternionRotateVector90DegreesNegZ()
        {
            Float4 vector = new Float4(10, 0, 0, 0);
            Quaternion quaternion = new Quaternion(new Float4(0, -1, 0, 1).Normalize());

            Float4 result = quaternion.RotateVector(vector);
            Assert.AreEqual(new Float4(0, 0, 10, 0), result);
        }

        [TestMethod]
        public void QuaternionRotateVector180Degrees()
        {
            Float4 vector = new Float4(10, 0, 0, 0);
            Quaternion quaternion = new Quaternion(new Float4(0, 0, 1, 0).Normalize());

            Float4 result = quaternion.RotateVector(vector);
            Assert.AreEqual(new Float4(-10, 0, 0, 0), result);
        }



        [TestMethod]
        public void QuaternionMultiplyEmptyQuaternion()
        {
            Quaternion quaternion1 = new Quaternion(new Float4(0, 0, 1, 1).Normalize());
            Quaternion quaternion2 = Quaternion.Default;

            Quaternion result = quaternion2.Multiply(quaternion1);
            Assert.AreEqual(new Float4(0, 0, 1, 1).Normalize(), result.ToFloat4());
        }

        [TestMethod]
        public void EmptyQuaternionMultiplyQuaternion90DegreesPosY()
        {
            Quaternion quaternion1 = Quaternion.Default;
            Quaternion quaternion2 = new Quaternion(new Float4(0, 0, 1, 1).Normalize());

            Quaternion result = quaternion2.Multiply(quaternion1);
            Assert.AreEqual(new Float4(0, 0, 1, 1).Normalize(), result.ToFloat4());
        }

        [TestMethod]
        public void EmptyQuaternionMultiplyQuaternion180Degrees()
        {
            Quaternion quaternion1 = Quaternion.Default;
            Quaternion quaternion2 = new Quaternion(new Float4(0, 0, 1, 0).Normalize());

            Quaternion result = quaternion2.Multiply(quaternion1);
            Assert.AreEqual(new Float4(0, 0, 1, 0).Normalize(), result.ToFloat4());
        }

        [TestMethod]
        public void QuaternionMultiplyQuaternion90DegreesPosY()
        {
            Quaternion quaternion1 = new Quaternion(new Float4(0, 0, 1, 1).Normalize());
            Quaternion quaternion2 = new Quaternion(new Float4(0, 0, 1, 1).Normalize());

            Quaternion result = quaternion2.Multiply(quaternion1);

            Quaternion expected = new Quaternion(new Float4(0, 0, 1, 0).Normalize());
            Assert.AreEqual(expected.X, result.X);
            Assert.AreEqual(expected.Y, result.Y);
            Assert.AreEqual(expected.Z, result.Z, Constants.Epsilon);
            Assert.AreEqual(expected.W, result.W);
        }

        [TestMethod]
        public void QuaternionMultiplyQuaternion180Degrees()
        {
            Quaternion quaternion1 = new Quaternion(new Float4(0, 0, 1, 1).Normalize());
            Quaternion quaternion2 = new Quaternion(new Float4(0, 0, 1, 0).Normalize());

            Quaternion result = quaternion1.Multiply(quaternion2);

            // Edge case
            Quaternion expected = new Quaternion(new Float4(0, 0, 1, -1).Normalize());
            Assert.AreEqual(expected.X, result.X);
            Assert.AreEqual(expected.Y, result.Y);
            Assert.AreEqual(expected.Z, result.Z, Constants.Epsilon);
            Assert.AreEqual(expected.W, result.W, Constants.Epsilon);
        }

        [TestMethod]
        public void QuaternionSameRotationDifferentNotation()
        {
            Float4 vector = new Float4(10, 10, 10, 0);

            Quaternion quaternion1 = new Quaternion(new Float4(0, 0, 1, -1).Normalize());
            Quaternion quaternion2 = new Quaternion(new Float4(0, 0, -1, 1).Normalize());

            Float4 rotated1 = quaternion1.RotateVector(vector);
            Float4 rotated2 = quaternion2.RotateVector(vector);

            Assert.AreEqual(rotated1, rotated2);
        }
    }
}
