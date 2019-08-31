using Ryne;
using Ryne.Utility.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class RotatorTests
    {
        [TestMethod]
        public void EmptyRotatorToQuaternionToRotator()
        {
            Rotator rotator = new Rotator();
            Quaternion quaternion = rotator.ToQuaternion();
            Rotator result = quaternion.ToRotator();

            Assert.AreEqual(rotator, result);
        }

        [TestMethod]
        public void RotatorXToQuaternion()
        {
            Rotator rotator = new Rotator(90.0f, 0, 0);
            Quaternion quaternion = rotator.ToQuaternion();
            Float4 result = quaternion.ToFloat4();

            Float4 expected = new Float4(0, 0, 1, 1).Normalize();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RotatorYToQuaternion()
        {
            Rotator rotator = new Rotator(0, 90.0f, 0);
            Quaternion quaternion = rotator.ToQuaternion();
            Float4 result = quaternion.ToFloat4();

            Float4 expected = new Float4(0, 1, 0, 1).Normalize();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RotatorZToQuaternion()
        {
            Rotator rotator = new Rotator(0, 0, 90.0f);
            Quaternion quaternion = rotator.ToQuaternion();
            Float4 result = quaternion.ToFloat4();

            Float4 expected = new Float4(1, 0, 0, 1).Normalize();
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RotatorXToQuaternionToRotator()
        {
            Rotator rotator = new Rotator(45.0f, 0, 0);
            Quaternion quaternion = rotator.ToQuaternion();
            Rotator result = quaternion.ToRotator();

            Assert.AreEqual(rotator.X, result.X, Constants.Epsilon);
            Assert.AreEqual(rotator.Y, result.Y, Constants.Epsilon);
            Assert.AreEqual(rotator.Z, result.Z, Constants.Epsilon);
        }

        [TestMethod]
        public void RotatorYToQuaternionToRotator()
        {
            Rotator rotator = new Rotator(0, 45.0f, 0);
            Quaternion quaternion = rotator.ToQuaternion();
            Rotator result = quaternion.ToRotator();

            Assert.AreEqual(rotator.X, result.X, Constants.Epsilon);
            Assert.AreEqual(rotator.Y, result.Y, Constants.Epsilon);
            Assert.AreEqual(rotator.Z, result.Z, Constants.Epsilon);
        }

        [TestMethod]
        public void RotatorZToQuaternionToRotator()
        {
            Rotator rotator = new Rotator(0, 0, 45.0f);
            Quaternion quaternion = rotator.ToQuaternion();
            Rotator result = quaternion.ToRotator();

            Assert.AreEqual(rotator.X, result.X, Constants.Epsilon);
            Assert.AreEqual(rotator.Y, result.Y, Constants.Epsilon);
            Assert.AreEqual(rotator.Z, result.Z, Constants.Epsilon);
        }

        [TestMethod]
        public void RotatorXYZToQuaternionToRotator()
        {
            Rotator rotator = new Rotator(45.0f, 45.0f, 45.0f);
            Quaternion quaternion = rotator.ToQuaternion();
            Rotator result = quaternion.ToRotator();

            Assert.AreEqual(rotator.X, result.X, Constants.Epsilon);
            Assert.AreEqual(rotator.Y, result.Y, Constants.Epsilon);
            Assert.AreEqual(rotator.Z, result.Z, Constants.Epsilon);
        }


        [TestMethod]
        public void RotatorXToQuaternion2()
        {
            Rotator rotator = new Rotator(45.0f, 0, 0);
            Quaternion quaternion = rotator.ToQuaternion();
            Float4 result = quaternion.ToFloat4();

            Float4 expected = new Quaternion(new Float3(0, 0, 1), 45.0f).ToFloat4();
            Assert.AreEqual(expected.X, result.X, Constants.Epsilon);
            Assert.AreEqual(expected.Y, result.Y, Constants.Epsilon);
            Assert.AreEqual(expected.Z, result.Z, Constants.Epsilon);
        }

        [TestMethod]
        public void RotatorYToQuaternion2()
        {
            Rotator rotator = new Rotator(0, 45.0f, 0);
            Quaternion quaternion = rotator.ToQuaternion();
            Float4 result = quaternion.ToFloat4();

            Float4 expected = new Quaternion(new Float3(0, 1, 0), 45.0f).ToFloat4();
            Assert.AreEqual(expected.X, result.X, Constants.Epsilon);
            Assert.AreEqual(expected.Y, result.Y, Constants.Epsilon);
            Assert.AreEqual(expected.Z, result.Z, Constants.Epsilon);
        }

        [TestMethod]
        public void RotatorZToQuaternion2()
        {
            Rotator rotator = new Rotator(0, 0, 45.0f);
            Quaternion quaternion = rotator.ToQuaternion();
            Float4 result = quaternion.ToFloat4();

            Float4 expected = new Quaternion(new Float3(1, 0, 0), 45.0f).ToFloat4();
            Assert.AreEqual(expected.X, result.X, Constants.Epsilon);
            Assert.AreEqual(expected.Y, result.Y, Constants.Epsilon);
            Assert.AreEqual(expected.Z, result.Z, Constants.Epsilon);
        }
    }
}
