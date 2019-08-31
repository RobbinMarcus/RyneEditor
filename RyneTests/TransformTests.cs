using Ryne;
using Ryne.Utility.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ryne.Scene;
using Ryne.Scene.Components;

namespace TestProject
{
    [TestClass]
    public class TransformTests
    {
        public static TransformComponent GameTransform;
        public static RyneTransformComponent EngineTransform;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ResetTransforms();
        }

        public static void ResetTransforms()
        {
            GameTransform = new TransformComponent();
            GameTransform.SetDefaults();
            EngineTransform = new RyneTransformComponent();
        }


        [TestMethod]
        public void TransformDefaults()
        {
            ResetTransforms();
            Assert.AreEqual(new Float4(0), GameTransform.Position);
            Assert.AreEqual(new Float4(0, 0, 0, 1), GameTransform.Rotation.ToFloat4());
            Assert.AreEqual(new Float4(0.5f), GameTransform.Pivot);
            Assert.AreEqual(new Float4(1), GameTransform.Scale);
        }

        [TestMethod]
        public void EngineTransformDefaults()
        {
            ResetTransforms();
            Assert.AreEqual(new Float4(0), EngineTransform.Position);
            Assert.AreEqual(new Float4(0, 0, 0, 1), EngineTransform.Rotation);
            Assert.AreEqual(new Float4(0.5f), EngineTransform.RelativePivot);
            Assert.AreEqual(new Float4(1), EngineTransform.Scale);
        }

        [TestMethod]
        public void TransformToEngineDefaults()
        {
            ResetTransforms();
            RyneTransformComponent converted = GameTransform.ToRenderComponent();
            Assert.AreEqual(converted.Position, EngineTransform.Position);
            Assert.AreEqual(converted.Rotation, EngineTransform.Rotation);
            Assert.AreEqual(converted.RelativePivot, EngineTransform.RelativePivot);
            Assert.AreEqual(converted.Scale, EngineTransform.Scale);
        }

        [TestMethod]
        public void TransformToEngine()
        {
            ResetTransforms();
            EngineTransform.Position = new Float4(10);
            EngineTransform.Rotation = new Quaternion(new Float3(0, 0, 1), 45).ToFloat4();
            EngineTransform.RelativePivot = new Float4(5);
            EngineTransform.Scale = new Float4(2);

            GameTransform.Position = new Float4(10);
            GameTransform.Rotation = new Quaternion(new Float3(0, 0, 1), 45);
            GameTransform.Pivot = new Float4(5);
            GameTransform.Scale = new Float4(2);

            RyneTransformComponent converted = GameTransform.ToRenderComponent();
            Assert.AreEqual(converted.Position, EngineTransform.Position);
            Assert.AreEqual(converted.Rotation, EngineTransform.Rotation);
            Assert.AreEqual(converted.RelativePivot, EngineTransform.RelativePivot);
            Assert.AreEqual(converted.Scale, EngineTransform.Scale);
        }


        // Tests with collision and transform components

        [TestMethod]
        public void RotateScaledCubeCenteredPivot()
        {
            ResetTransforms();
            CollisionComponent collision = new CollisionComponent();
            collision.SetCube(new Cube(new Float4(0.5f), new Float4(0.5f), Quaternion.Default));

            GameTransform.Scale = new Float4(10);
            GameTransform.Rotation = new Quaternion(new Float3(0, 0, 1), 90.0f);
            GameTransform.Pivot = new Float4(5);
            var converted = collision.ExtractCube(GameTransform);

            Assert.AreEqual(new Float4(5), converted.Center);
            Assert.AreEqual(new Float4(5), converted.Size);
            Assert.AreEqual(new Quaternion(new Float3(0, 0, 1), 90.0f).ToFloat4(), converted.Rotation.ToFloat4());
        }

        [TestMethod]
        public void RotateScaledCubeCustomPivot()
        {
            ResetTransforms();
            CollisionComponent collision = new CollisionComponent();
            collision.SetCube(new Cube(new Float4(0.5f), new Float4(0.5f), Quaternion.Default));

            GameTransform.Scale = new Float4(10);
            GameTransform.Rotation = new Quaternion(new Float3(0, 0, 1), 90.0f);
            GameTransform.Pivot = new Float4(0);
            var converted = collision.ExtractCube(GameTransform);

            // Precision
            Assert.AreEqual(-5.0f, converted.Center.X, Constants.Epsilon);
            Assert.AreEqual(5.0f, converted.Center.Y, Constants.Epsilon);
            Assert.AreEqual(5.0f, converted.Center.Z, Constants.Epsilon);

            Assert.AreEqual(new Float4(5), converted.Size);
            Assert.AreEqual(new Quaternion(new Float3(0, 0, 1), 90.0f).ToFloat4(), converted.Rotation.ToFloat4());
        }
    }
}
