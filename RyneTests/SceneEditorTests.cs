using Ryne.Entities;
using Ryne.Scene.Components;
using Ryne.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class SceneEditorTests
    {
        public static Entity TestEntity;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            TestGlobal.Initialize();

            TestEntity = new Entity();
            TestEntity.AddComponent<TransformComponent>();
            Global.EntityManager.AddEntity(TestEntity);
        }

        [ClassCleanup]
        public static void Dispose()
        {
            TestGlobal.Dispose();
        }

        public static void ResetTestEntity()
        {
            TestEntity.Transform.SetDefaults();
        }

        [TestMethod]
        [TestCategory("ApplicationTests")]
        public void SelectEntity()
        {
            ResetTestEntity();

            TestGlobal.Editor.SelectEntity(null);
            TestGlobal.Editor.SelectEntity(TestEntity);
            Assert.AreEqual(TestEntity, TestGlobal.Editor.SelectedEntity);
        }
    }
}
