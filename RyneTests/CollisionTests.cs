using Ryne;
using Ryne.Scene;
using Ryne.Utility.Math;
using Ryne.Scene.Systems;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class CollisionTests
    {
        [TestMethod]
        public void AABBAABBNoOverlap()
        {
            AABB aabb1 = new AABB(new Float4(0), new Float4(10));
            AABB aabb2 = new AABB(new Float4(10), new Float4(20));
            SeparatingAxisTheorem sat = new SeparatingAxisTheorem();
            var b1 = new SatBodyData();
            var b2 = new SatBodyData();
            b1.CreateFrom(aabb1);
            b2.CreateFrom(aabb2);
            var data = sat.Test(b1, b2);
            Assert.AreEqual(false, data.Intersecting);
        }

        [TestMethod]
        public void AABBAABBPosYOverlap()
        {
            AABB aabb1 = new AABB(new Float4(0, 9, 0, 0), new Float4(10, 19, 10, 10));
            AABB aabb2 = new AABB(new Float4(0), new Float4(10));
            SeparatingAxisTheorem sat = new SeparatingAxisTheorem();
            var b1 = new SatBodyData();
            var b2 = new SatBodyData();
            b1.CreateFrom(aabb1);
            b2.CreateFrom(aabb2);
            var data = sat.Test(b1, b2);
            Assert.AreEqual(true, data.Intersecting);
            Assert.AreEqual(1, data.Depth);
            Assert.AreEqual(new Float3(0, 1, 0), data.Normal);
        }

        [TestMethod]
        public void AABBAABBNegYOverlap()
        {
            AABB aabb1 = new AABB(new Float4(0, -9, 0, 0), new Float4(10, 1, 10, 10));
            AABB aabb2 = new AABB(new Float4(0), new Float4(10));
            SeparatingAxisTheorem sat = new SeparatingAxisTheorem();
            var b1 = new SatBodyData();
            var b2 = new SatBodyData();
            b1.CreateFrom(aabb1);
            b2.CreateFrom(aabb2);
            var data = sat.Test(b1, b2);
            Assert.AreEqual(true, data.Intersecting);
            Assert.AreEqual(1, data.Depth);
            Assert.AreEqual(new Float3(0, -1, 0), data.Normal);
        }

        [TestMethod]
        public void CubeCubeNoOverlap()
        {
            Cube cube1 = new Cube(new Float4(0), new Float4(5), Quaternion.Default);
            Cube cube2 = new Cube(new Float4(10), new Float4(5), Quaternion.Default);
            SeparatingAxisTheorem sat = new SeparatingAxisTheorem();
            var b1 = new SatBodyData();
            var b2 = new SatBodyData();
            b1.CreateFrom(cube1);
            b2.CreateFrom(cube2);
            var data = sat.Test(b1, b2);
            Assert.AreEqual(false, data.Intersecting);
        }

        [TestMethod]
        public void CubeCubeOverlap()
        {
            Cube aabb1 = new Cube(new Float4(0, 9, 0, 0), new Float4(5), Quaternion.Default);
            Cube aabb2 = new Cube(new Float4(0), new Float4(5), Quaternion.Default);
            SeparatingAxisTheorem sat = new SeparatingAxisTheorem();
            var b1 = new SatBodyData();
            var b2 = new SatBodyData();
            b1.CreateFrom(aabb1);
            b2.CreateFrom(aabb2);
            var data = sat.Test(b1, b2);
            Assert.AreEqual(true, data.Intersecting);
            Assert.AreEqual(1, data.Depth);
            Assert.AreEqual(new Float3(0, 1, 0), data.Normal);
        }

        [TestMethod]
        public void SphereAABBOverlap()
        {
            Sphere sphere = new Sphere(new Float3(15, 0, 0), 10);
            AABB aabb = new AABB(new Float4(0), new Float4(10));
            var data = CollisionSystem.SphereAABBIntersection(sphere, aabb);
            Assert.AreEqual(true, data.Intersecting);
            Assert.AreEqual(5, data.Depth);
            Assert.AreEqual(new Float3(1, 0, 0), data.Normal);
        }

        [TestMethod]
        public void SphereCubeOverlap()
        {
            Sphere sphere = new Sphere(new Float3(15, 0, 0), 10);
            Cube cube = new Cube(new Float4(0), new Float4(10), Quaternion.Default);
            var data = CollisionSystem.SphereCubeIntersection(sphere, cube);
            Assert.AreEqual(true, data.Intersecting);
            Assert.AreEqual(5, data.Depth);
            Assert.AreEqual(new Float3(1, 0, 0), data.Normal);
        }

        [TestMethod]
        public void SphereSphereOverlap()
        {
            Sphere sphere1 = new Sphere(new Float3(0), 10);
            Sphere sphere2 = new Sphere(new Float3(15, 0, 0), 10);
            var data = CollisionSystem.SphereSphereIntersection(sphere1, sphere2);
            Assert.AreEqual(true, data.Intersecting);
            Assert.AreEqual(5, data.Depth);
            Assert.AreEqual(new Float3(-1, 0, 0), data.Normal);
        }
    }
}
