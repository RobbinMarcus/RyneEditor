using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ryne.Entities;
using Ryne.Gui;
using Ryne.Gui.GuiElements;
using Ryne.Input;
using Ryne.Scene;
using Ryne.Scene.Components;
using Ryne.Utility;
using Ryne.Utility.Actions;
using Ryne.Utility.Collections;
using Ryne.Utility.Math;

namespace Ryne.GameStates
{
    public enum EditMode
    {
        Translate,
        Rotate,
        Scale
    }

    public enum EditAxis
    {
        X,
        Y,
        Z,
        All
    }

    public class SceneEditor : GameState
    {
        public SceneEditorGui Gui { get; }

        // Copy of all entities to reset the world after game testing
        private EntityStorage EditorEntities;

        public RyneCamera EditorCamera { get; }
        private Entity EditorCameraEntity;
        private readonly Controller EditorController;
        private readonly EditorGridController EntityController;

        public EditMode CurrentEditMode { get; private set; }
        public EditAxis CurrentEditAxis { get; private set; }

        public bool SharedPivotRotationMode
        {
            get => EntityController.SharedPivotRotationMode;
            set => EntityController.SharedPivotRotationMode = value;
        }


        private readonly SceneData SceneData;
        public string SceneName => SceneData.SceneName;

        public Entity SelectedEntity => EntityController.ControlledEntity;
        public List<Entity> SelectedEntities => EntityController.ControlledEntities;


        private List<Entity> CopyBuffer;

        private RyneDelegate OnSizeChangedDelegate;

        private readonly Queue<IAction> UndoBuffer;
        private const int UndoBufferMaxSize = 10;


        public SceneEditor(StateManager manager) : base(manager)
        {
            SceneRenderData = new RyneSceneRenderData();
            SceneData = new SceneData(this);
            Gui = new SceneEditorGui(this);
            EditorController = new Controller { ClampVerticalRotation = true };
            EntityController = new EditorGridController(this);
            EditorCamera = new RyneCamera(Global.Config.Width, Global.Config.Height);
            UndoBuffer = new Queue<IAction>();

            CurrentEditMode = EditMode.Translate;
            CurrentEditAxis = EditAxis.All;
        }

        public override void Initialize()
        {
            base.Initialize();
            Gui.Initialize(SceneRenderData);

            if (!Global.Config.NoRendering)
            {
                SceneRenderData.SetRenderer(RyneRendererType.RayTracer);
                SceneRenderData.SetRenderCamera(EditorCamera);
            }

            EntityController.SetInputMapping(new TranslationInputMapping(EditorCamera, CurrentEditAxis));
            EntityController.OnEntitiesChanged = OnEntitiesChanged;

            OnSizeChangedDelegate = new RyneDelegate(OnSizeChanged);
            Global.Application.SetOnSizeChanged(OnSizeChangedDelegate);

            if (string.IsNullOrEmpty(SceneData.SceneName))
            {
                if (!SceneData.Exists("Default"))
                {
                    NewScene("Default");
                }
                else
                {
                    LoadScene("Default");
                }
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            Gui.Update(dt);

            // Update here to capture all key inputs, even when in gui
            EditorController.Update(dt);
            if (EditorController.Changed())
            {
                OnEditorControllerChanged(dt);
            }

            if (Global.Application.KeyPressed((int)RyneKey.Escape))
            {
                if (Gui.AnyActivePopups)
                {
                    Gui.ClearWindows();
                }
                else
                {
                    var popup = new PopupGui(Gui, "Are you sure you want to quit?");
                    popup.Callback += result =>
                    {
                        // Save and exit
                        SceneData.Flush();
                        Logger.Flush();
                        StateManager.ExitCurrentState();
                    };
                    // TODO: hack for width based on title. Need to check auto width
                    popup.MinWidth = 200.0f;
                    Gui.AddPopup(popup);
                }

                return;
            }
            if (!Global.Application.IsRunning)
            {
                // Save and exit
                SceneData.Flush();
                Logger.Flush();
                StateManager.ExitCurrentState();
            }

            // Only application code below
            if (Gui != null && Gui.IsMouseInGui())
            {
                return;
            }

            EntityController.Update(dt);
            if (EntityController.Changed())
            {
                foreach (var entity in EntityController.ControlledEntities.Where(x => x.RegisteredBackend))
                {
                    SceneRenderData.UpdateTransform(entity.RenderId, entity.Transform.ToRenderComponent());
                }
            }

            if (EditorController.CaptureMouse)
            {
                return;
            }

            if (Global.Application.MouseReleased((int)RyneMouse.Button0) && !EntityController.MovedMouseSinceStart)
            {
                var cursorPosition = Global.Application.GetCursorPosition(true);
                QueryScene(cursorPosition);
            }
        }

        public override void Draw(float dt)
        {
            SceneRenderData.RenderScene();
            Gui.Draw(dt);
        }

        public override void PostFrame()
        {
            SceneRenderData?.PostFrame();
        }

        public override void Destroy()
        {
            Global.Application.ClearCallbacks();
            OnSizeChangedDelegate?.Free();
            Gui?.Destroy();
            Gui?.Dispose();
            EditorCamera?.Dispose();
            SceneRenderData.Dispose();
            base.Destroy();
        }

        public void ChangeEditMode(EditMode newMode)
        {
            if (EditorController.CaptureMouse || EntityController.CaptureMouse)
            {
                return;
            }

            CurrentEditMode = newMode;
            switch (CurrentEditMode)
            {
                case EditMode.Translate: EntityController.SetInputMapping(new TranslationInputMapping(EditorCamera, CurrentEditAxis)); break;
                case EditMode.Rotate: EntityController.SetInputMapping(new RotationInputMapping(EditorCamera, CurrentEditAxis)); break;
                case EditMode.Scale: EntityController.SetInputMapping(new ScaleInputMapping(EditorCamera, CurrentEditAxis)); break;
            }
        }

        public void ChangeEditAxis(EditAxis newAxis)
        {
            if (EditorController.CaptureMouse || EntityController.CaptureMouse)
            {
                return;
            }

            CurrentEditAxis = CurrentEditAxis == newAxis ? EditAxis.All : newAxis;

            if (EntityController.ActiveMapping is ObjectManipulationMapping mapping)
            {
                mapping.UpdateMapping(EditorCamera, CurrentEditAxis);
                EntityController.SetInputMapping(mapping);
            }
        }

        public void ChangeRenderMode(RyneRenderMode newMode)
        {
            SceneRenderData.ChangeRendermode(newMode);
        }

        public void CopySelectedEntities()
        {
            CopyBuffer = new List<Entity>();
            if (EntityController.ControlledEntities.Count > 0)
            {
                CopyBuffer = EntityController.ControlledEntities.ToList();
            }
        }

        public void PasteSelectedEntities()
        {
            EntityController.ClearSelection();
            if (CopyBuffer.Count > 0)
            {
                foreach (var entity in CopyBuffer)
                {
                    var serialized = entity.Serialize();
                    var deserialized = Entity.Deserialize(serialized);

                    ref var newEntity = ref Global.EntityManager.Entities[deserialized.EntityId];
                    newEntity.Name = Helpers.GetUniqueName(newEntity.Name);
                    // Flag as changed so it will be saved
                    newEntity.SetChangedInEditor(true);

                    EntityController.AddEntity(newEntity);
                }
            }
        }

        // Push an IAction to the buffer so we can undo the last action
        public void AddUndoAction(IAction action)
        {
            UndoBuffer.Enqueue(action);
            if (UndoBuffer.Count > UndoBufferMaxSize)
            {
                UndoBuffer.Dequeue();
            }
        }

        public void UndoLastAction()
        {
            if (UndoBuffer.Count <= 0) return;

            var temp = UndoBuffer.ToList();
            UndoBuffer.Clear();

            var action = temp.Last();
            action.Undo();

            temp.RemoveLast();
            foreach (var action1 in temp)
            {
                UndoBuffer.Enqueue(action1);
            }
        }


        // All Gui commands

        public void NewScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Logger.Warning("Empty scene name");
                return;
            }

            SceneData.New(sceneName);
            AddCameraEntity();
        }

        public void LoadScene(string sceneName)
        {
            if (SceneData.SceneName == sceneName)
            {
                Logger.Log($"Already in scene {sceneName}.");
                return;
            }

            SceneData.Load(sceneName);
            AddCameraEntity();
        }

        public void SaveScene()
        {
            SceneData.Flush();
        }

        public void OpenRenderStatistics()
        {
            SceneRenderData.OpenRenderStatistics();
        }

        public void RenderStatistics()
        {
            SceneRenderData.RenderStatistics();
        }

        public void VoxelizeModel(string filename, int levels, int voxelizeFlags = 0, string outputFilename = "")
        {
            if (string.IsNullOrEmpty(outputFilename))
            {
                outputFilename = new FileInfo(filename).Name;
            }

            var name = Path.GetFileNameWithoutExtension(outputFilename);
            outputFilename = Global.Config.WorkingDirectory + "VoxelModels/" + name + ".dag";

            RyneTools tools = new RyneTools();
            tools.VoxelizeModel(filename, outputFilename, levels, voxelizeFlags);
        }

        public void LoadBackgroundHdri(string filename)
        {
            if (!new FileInfo(Global.Config.WorkingDirectory + filename).Exists)
            {
                Logger.Warning($"Tried to load HDRI from non existing file {filename}");
                return;
            }

            SceneRenderData.SetBackgroundTexture(filename);
            if (SceneData != null)
            {
                SceneData.BackgroundHdri = filename;
            }
        }

        public void TestGame()
        {
            EditorController.ControlledEntities.Clear();
            EntityController.ControlledEntities.Clear();
            Gui.ClearWindows();

            // Store entities for restore
            EditorEntities = new EntityStorage(Global.EntityManager.Data);
            // TODO: Entity is class type, copied as a reference
            //Global.EntityManager.Entities[0].SetIsDestroyed(true);

            Global.Application.ClearCallbacks();
            var game = new Game(this) { OnExitCallback = RestoreEditorStateAfterGame };
            StateManager.Add(game);
        }

        public void AddEntity(Entity entity)
        {
            Global.EntityManager.AddEntity(entity);
            SceneData.Flush();
        }

        public void DeleteEntity(Entity entity)
        {
            AddUndoAction(new UndoDeleteAction(entity.Serialize()));

            entity.SetChangedInEditor(false);
            Global.EntityManager.DeleteEntity(ref entity);
            SceneData.DeleteEntityFromScene(entity);
        }

        public void TryRenameEntity()
        {
            if (SelectedEntity != null)
            {
                var popup = new PopupGui(Gui, "Rename");
                popup.AddElement(new InputTextGuiElement("Name"));
                popup.Callback = result =>
                {
                    var name = result.GetTextInputValues()["Name"];
                    RenameEntity(name, SelectedEntity);
                };
                Gui.AddPopup(popup);
            }
        }

        public void DeleteActiveEntities(bool ensureNoActiveWindows = true)
        {
            if (ensureNoActiveWindows && Gui.AnyActivePopups)
            {
                return;
            }

            foreach (var entity in EntityController.ControlledEntities)
            {
                DeleteEntity(entity);
            }

            // Clear selection
            SelectEntity(null);
        }

        public void SelectEntity(Entity entity, bool addToSelection = false)
        {
            Gui.ClearWindows();

            if (entity is null && !addToSelection)
            {
                EntityController.ControlledEntities.Clear();
                return;
            }

            if (EntityController.ActiveMapping is ObjectManipulationMapping mapping)
            {
                mapping.UpdateMapping(EditorCamera, CurrentEditAxis);
                EntityController.SetInputMapping(mapping);
            }

            if (addToSelection)
            {
                EntityController.AddEntity(entity);
            }
            else
            {
                EntityController.ControlEntity(entity);
            }
        }

#if DEBUG
        public unsafe void DrawDebugWindow()
        {
            if (!EditorCameraEntity.Initialized || Gui is null)
            {
                return;
            }

            if (Gui.Begin("EditorDebug"))
            {
                var size = Global.Application.GetRenderSize();
                var screenSize = Global.Application.GetScreenSize();
                Gui.Text($"Render size: {size.X} {size.Y}");
                Gui.Text($"Screen size: {screenSize.X} {screenSize.Y}");

                fixed (Float4* position = &EditorCameraEntity.Transform.Position)
                {
                    if (Gui.InputFloat4("Camera Position", position))
                    {
                        EditorCamera.Position = *position;
                        EditorCamera.Update(0.0f);
                    }
                }

                Float3 rotation = EditorCameraEntity.Transform.Rotation.ToRotator().ToFloat3();
                if (Gui.InputFloat3("Camera Rotation", &rotation))
                {
                    //EditorCameraEntity.Transform.Rotation = new Rotator(rotation).ToQuaternion();
                }

                Float4 quatRotation = EditorCameraEntity.Transform.Rotation.ToFloat4();
                if (Gui.InputFloat4("Camera Quaternion", &quatRotation))
                {
                    //EditorCameraEntity.Transform.Rotation = new Quaternion(quatRotation);
                }
            }
            Gui.End();
        }
#endif

        public void HideGui()
        {
            Gui.HideUI = !Gui.HideUI;
        }

        public void RenderScreenToImage(string filename)
        {
            SceneRenderData.SaveScreenToImage(filename);
        }

        public void UpdateAlignmentValues()
        {
            EntityController.UpdateAlignmentValues();
        }

        public void RenameEntity(string name, Entity entity)
        {
            if (entity != null)
            {
                SceneData.DeleteEntityFromScene(entity);

                entity.Name = name;
                entity.SetChangedInEditor(true);
            }
        }




        private void RestoreEditorStateAfterGame()
        {
            EntityController.ControlledEntities.Clear();

            Global.EntityManager.RestoreEntities(EditorEntities);
            Global.Application.SetOnSizeChanged(OnSizeChangedDelegate);

            // TODO: array copying of entities is pass by reference, so game entities are the same as editor entities
            // Will need some way of restoring entities after game testing took place
            foreach (var entity in Global.EntityManager.Entities)
            {
                entity.SetIsDestroyed(false);
            }

            EditorController.ControlEntity(EditorCameraEntity);
            SceneRenderData.SetRenderCamera(EditorCamera);
        }

        private void QueryScene(Float2 viewportPixel)
        {
            Float4 direction = EditorCamera.Unproject(viewportPixel);
            RyneRay ray = new RyneRay(new Float3(EditorCamera.Position), new Float3(direction), 5000.0f, 0.0f);

            // TODO: keep RyneRayTracingFunctions as private member variable?
            RyneRayTracingFunctions rayTracer = new RyneRayTracingFunctions(SceneRenderData);
            var result = rayTracer.Trace(ray);

            bool addToSelection = Global.Application.KeyDown((int)RyneKey.LeftControl) ||
                                  Global.Application.KeyDown((int)RyneKey.RightControl);

            if (result.Distance < Constants.RayMaxDistance)
            {
                // TODO: O(n)
                Entity entity = Global.EntityManager.Entities.FirstOrDefault(x => x.RenderId == result.ObjectId);
                if (entity != null)
                {
                    Logger.Log($"Found entity from QueryScene with RenderId = {result.ObjectId}");
                    SelectEntity(entity, addToSelection);
                }
            }
            else
            {
                SelectEntity(null, addToSelection);
            }
        }

        private void OnSizeChanged()
        {
            var size = Global.Application.GetRenderSize();
            EditorCamera.UpdateResolution(new Float2(size.X, size.Y));
        }

        private void OnEntitiesChanged()
        {
            foreach (var entity in EntityController.ControlledEntities)
            {
                entity.SetChangedInEditor(true);
            }
        }

        private void OnCameraViewUpdated()
        {
            if (EntityController.ActiveMapping is TranslationInputMapping inputMapping)
            {
                inputMapping.UpdateMapping(EditorCamera, CurrentEditAxis);
            }
        }

        private void AddCameraEntity()
        {
            EditorCameraEntity = Global.EntityManager.Entities.FirstOrDefault(x => x.Name == "EditorCameraEntity");
            if (EditorCameraEntity is null)
            {
                EditorCameraEntity = new Entity { Name = "EditorCameraEntity" };
                EditorCameraEntity.AddComponent<TransformComponent>();
                Global.EntityManager.AddEntity(EditorCameraEntity);
            }

            EditorCameraEntity.SetFlag(EntityFlag.EditorNotEditable, true);

            EditorController.SetInputMapping(new EditorInputMapping(this));
            EditorController.ControlEntity(EditorCameraEntity);

            OnEditorControllerChanged(0.0f);
        }

        private void OnEditorControllerChanged(float dt)
        {
            EditorCameraEntity.SetChangedInEditor(true);
            if (EditorCamera == null)
                return;

            EditorCamera.Position = EditorCameraEntity.Transform.Position;
            EditorCamera.Rotation = EditorCameraEntity.Transform.Rotation.ToRotator().ToFloat3();
            if (System.Math.Abs(EditorController.FovDelta) > Constants.Epsilon)
            {
                float fov = RyneMath.RadiansToDegrees(EditorCamera.FieldOfView) + EditorController.FovDelta * dt;
                EditorCamera.FieldOfView =
                    RyneMath.DegreesToRadians(System.Math.Max(8.0f, System.Math.Min(fov, 100.0f)));
            }

            EditorCamera.Update(dt);
            OnCameraViewUpdated();
        }
    }
}
