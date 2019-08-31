using System;
using System.IO;
using Ryne;
using Ryne.GameStates;
using Ryne.Utility;

namespace TestProject
{
    class TestGlobal
    {
        private static string SavedWorkingDirectory;

        public static EngineCore Engine;
        public static SceneEditor Editor;

        public static void Initialize()
        {
            Global.Config.NoRendering = true;
            SavedWorkingDirectory = Global.Config.WorkingDirectory;
            Global.Config.WorkingDirectory = "Content/";

            Engine = new EngineCore();
            Engine.Initialize("Ryne");

            Editor = new SceneEditor(Engine.StateManager);

            var unitTestSceneFile = "Scenes/UnitTestScene/UnitTestScene.fls";
            var unitTestDirectory = new FileInfo(Global.Config.WorkingDirectory + unitTestSceneFile).DirectoryName;
            if (Directory.Exists(unitTestDirectory))
            {
                Directory.Delete(unitTestDirectory, true);
            }

            Editor.NewScene("UnitTestScene");
            Engine.StateManager.Add(Editor);
            Engine.Update(0.0f);
        }

        public static void Dispose()
        {
            Engine.StateManager.ExitCurrentState();
            Engine.Update(0.0f);
            Engine.Dispose();

            GC.Collect();

            // Reset config changes
            Global.Config.NoRendering = false;
            Global.Config.WorkingDirectory = SavedWorkingDirectory;
        }
    }
}
