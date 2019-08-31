using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ryne.Entities;
using Ryne.GameStates;
using Ryne.Utility;

namespace Ryne.Scene
{
    // Manages the IO of scenes
    public class SceneData
    {
        public string SceneName { get; private set; }
        public string BackgroundHdri { get; set; }

        private readonly SceneEditor Editor;
        private string Folder;

        private readonly List<string> DeletedEntities;

        public SceneData(SceneEditor editor)
        {
            Editor = editor;
            BackgroundHdri = "";
            DeletedEntities = new List<string>();
        }

        public void Flush()
        {
            // Delete all entity files marked for deletion
            foreach (var deletedEntity in DeletedEntities)
            {
                var filename = GetEntityFilename(deletedEntity);
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }
            DeletedEntities.Clear();

            // Save all changed entities
            var entityIds = Global.EntityManager.Entities.Where(x => x.ChangedInEditor).Select(x => x.EntityId).ToArray();

            // TODO: parallel foreach
            foreach (var entityId in entityIds)
            {
                ref var entity = ref Global.EntityManager.Entities[entityId];
                if (entity.Destroyed)
                {
                    continue;
                }

                Logger.Log($"SceneData: Saving {entity.Name}");

                var filename = GetEntityFilename(entity.Name);
                StreamWriter writer = new StreamWriter(filename, false);
                writer.Write(entity.Serialize());
                writer.Flush();
                writer.Close();

                entity.SetChangedInEditor(false);
            }

            var sceneFile = Folder + SceneName + ".fls";
            StreamWriter sceneWriter = new StreamWriter(sceneFile, false);
            sceneWriter.Write(BackgroundHdri);
            sceneWriter.Flush();
            sceneWriter.Close();
        }

        public void New(string sceneName, bool load = true)
        {
            var folder = GetFolder(sceneName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (File.Exists(folder + sceneName + ".fls"))
            {
                Logger.Error($"Tried to create scene: {sceneName}, but scene already exists.");
                return;
            }

            var stream = File.Create(folder + sceneName + ".fls");
            stream.Close();
            
            if (load)
            {
                Load(sceneName);
            }
        }

        public void Load(string sceneName)
        {
            var sceneFile = GetFolder(sceneName) + sceneName + ".fls";
            var fileInfo = new FileInfo(sceneFile);
            if (!fileInfo.Exists)
            {
                Logger.Error($"Could not load scene: {sceneName}. File doesn't exist.");
                return;
            }

            SceneName = sceneName;
            BackgroundHdri = "";
            UpdateFolder();

            Global.EntityManager.Clear();
            var activeState = Global.StateManager.GetCurrentState();
            activeState?.SceneRenderData.Clear();

            StreamReader sceneReader = new StreamReader(sceneFile);
            string sceneContent = sceneReader.ReadToEnd();
            sceneReader.Close();

            // Read scene file
            var lines = sceneContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (lines.Count > 0)
            {
                BackgroundHdri = lines[0];
                if (!string.IsNullOrEmpty(BackgroundHdri))
                {
                    Editor.LoadBackgroundHdri(BackgroundHdri);
                }
            }

            // TODO: Parallel?
            var files = Directory.GetFiles(Folder);
            foreach (var file in files)
            {
                // Skip scene file
                if (new FileInfo(file).Extension == ".fls")
                {
                    continue;
                }

                StreamReader reader = new StreamReader(file);
                string content = reader.ReadToEnd();
                reader.Close();

                var entity = Entity.Deserialize(content);
                Logger.Log($"SceneData: Loaded {entity.Name}");
            }
        }

        public bool Exists(string sceneName)
        {
            var sceneFile = GetFolder(sceneName) + sceneName + ".fls";
            var fileInfo = new FileInfo(sceneFile);
            return fileInfo.Exists;
        }

        public void DeleteEntityFromScene(Entity entity)
        {
            DeletedEntities.Add(entity.Name);
        }

        private void UpdateFolder()
        {
            Folder = GetFolder(SceneName);
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
            Logger.Log($"SceneData: Changed scene name to {SceneName}");
        }

        private string GetFolder(string sceneName)
        {
            return Global.Config.WorkingDirectory + "Scenes/" + sceneName + "/";
        }

        private string GetEntityFilename(string entityName)
        {
            return Folder + entityName;
        }
    }
}
