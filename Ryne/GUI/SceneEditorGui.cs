using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ryne.Entities;
using Ryne.GameStates;
using Ryne.Gui.GuiElements;
using Ryne.Gui.Windows;
using Ryne.Utility;

namespace Ryne.Gui
{
    public class SceneEditorGui : ImGuiWrapper
    {
        private readonly SceneEditor Editor;

        private readonly List<WindowGui> ActiveWindows;
        private readonly List<PopupGui> ActivePopups;

        private readonly List<WindowGui> WindowsToAdd;

        public bool RenderDebugVisualization
        {
            get => RenderDebugVisualizationToggle;
            set => RenderDebugVisualizationToggle = value;
        }

        public bool AnyActivePopups => ActivePopups.Count + ActiveWindows.Count > 0;

        public RyneSceneRenderData SceneData => Editor.SceneRenderData;

        private bool RenderSettingsToggle;
        private bool RenderDebugVisualizationToggle;
        private bool FpsWindowToggle;

        public int RenderCount { get; set; }

        public SceneEditorGui(SceneEditor editor)
        {
            Editor = editor;
            ActiveWindows = new List<WindowGui>();
            ActivePopups = new List<PopupGui>();

            WindowsToAdd = new List<WindowGui>();

            RenderDebugVisualizationToggle = false;
            RenderSettingsToggle = false;
            FpsWindowToggle = false;

            RenderCount = GetCurrentRenderCount();
        }

        public override void Initialize(RyneSceneRenderData renderData)
        {
            base.Initialize(renderData);
        }

        public override void Draw(float dt)
        {
            if (HideUI)
            {
                base.Draw(dt);
                return;
            }

            PushFont(GetFont("Roboto16"));
            EditorDockLayout();

            // Render the default space where all windows can dock
            DockSpace();

            DrawMenuBar();
            DrawToolBar();
            DrawPropertiesBar();
            DrawLog();
            DrawExtensionWindows();

            RenderGenericWindows();
            RenderPopups();

            // Render statistics window if open
            Editor.RenderStatistics();
#if DEBUG
            // Debug SceneEditor
            Editor.DrawDebugWindow();
#endif

            PopFont();

            if (RenderDebugVisualization)
            {
                if (Editor.TraceDebugRay)
                {
                    Editor.SceneRenderData.RenderDebugRays(this);
                }

                DebugRenderVisualization(Editor.EditorCamera);
            }

            base.Draw(dt);
        }

        public void ClearWindows()
        {
            CloseGenericWindows();
            ActiveWindows.Clear();
            ActivePopups.Clear();
        }

        public void AddWindow(WindowGui window)
        {
            if (window is PopupGui popup)
            {
                Logger.Warning($"Added popup to window list: popup {popup.WindowTitle} will be lost");
                return;
            }

            if (ActiveWindows.Any(x => x.WindowTitle == window.WindowTitle))
            {
                ActiveWindows.RemoveAll(x => x.WindowTitle == window.WindowTitle);
            }

            WindowsToAdd.Add(window);
        }

        public void AddPopup(PopupGui popup)
        {
            // Don't duplicate existing popups
            if (ActivePopups.Any(x => x.WindowTitle == popup.WindowTitle))
            {
                return;
            }

            ActivePopups.Add(popup);
        }


        private void DrawMenuBar()
        {
            if (BeginMainMenuBar())
            {
                if (BeginMenu("File"))
                {
                    if (MenuItem("New scene"))
                    {
                        var window = new PopupGui(this, "New Scene");
                        window.AddElement(new InputTextGuiElement("Scene name"));
                        window.Callback += result => Editor.NewScene(result.GetTextInputValues()["Scene name"]);
                        AddPopup(window);
                    }

                    if (MenuItem("Save scene"))
                    {
                        Editor.SaveScene();
                    }

                    if (MenuItem("Load scene"))
                    {
                        var window = new FileExplorerGui(this, "Scenes", ".fls", "Load scene");
                        window.Callback += result =>
                        {
                            var fileName = ((FileExplorerGui)result).SelectedFile;
                            var sceneName = Path.GetFileNameWithoutExtension(fileName);
                            Editor.LoadScene(sceneName);
                        };
                        AddPopup(window);
                    }

                    Separator();

                    if (MenuItem("Save current render to image"))
                    {
                        if (!Directory.Exists("Renders"))
                        {
                            Directory.CreateDirectory("Renders");
                        }

                        RenderCount++;
                        Editor.RenderScreenToImage("Renders/Render" + RenderCount + ".tga");
                    }

                    Separator();

                    if (MenuItem("Change background HDRI"))
                    {
                        var extensions = Global.ResourceManager.GetSupportedExtensions(RyneResourceType.ResourceTypeTexture);
                        var window = new FileExplorerGui(this, "Backgrounds", extensions, "Select background HDRI");
                        window.Callback += result => Editor.LoadBackgroundHdri(((FileExplorerGui)result).SelectedFile);
                        AddPopup(window);
                    }

                    EndMenu();
                }

                // TODO: create a debug render mode instead?
                if (BeginMenu("View"))
                {
                    if (MenuItem("Render statistics"))
                    {
                        Editor.OpenRenderStatistics();
                    }

                    if (MenuItem("FPS", ref FpsWindowToggle)) { }
                    if (MenuItem("Render settings", ref RenderSettingsToggle)) { }

                    if (MenuItem("Camera settings"))
                    {
                        Editor.EditorCamera.OpenEditorWindow(this);
                    }

#if DEBUG
                    Separator();
                    if (MenuItem("Debug rays", ref Editor.TraceDebugRay)) { }
                    if (MenuItem("Render debug visualization", ref RenderDebugVisualizationToggle)) { }
#endif

                    EndMenu();
                }

                if (BeginMenu("Game"))
                {
                    if (MenuItem("Test"))
                    {
                        Editor.TestGame();
                    }

                    EndMenu();
                }

                if (BeginMenu("Add"))
                {
                    if (MenuItem("Entity"))
                    {
                        AddEntityWindow();
                    }

                    EndMenu();
                }

                EndMainMenuBar();
            }
        }

        private void DrawToolBar()
        {
            if (Begin("Tools"))
            {
                if (CollapsingHeader("Editor", true))
                {
                    Text("Current edit mode:");
                    int editMode = (int)Editor.CurrentEditMode;
                    if (RadioButton("[T]ranslate", ref editMode, 0)) Editor.ChangeEditMode(EditMode.Translate);
                    SameLine();
                    if (RadioButton("[R]otate", ref editMode, 1)) Editor.ChangeEditMode(EditMode.Rotate);
                    SameLine();
                    if (RadioButton("[S]cale", ref editMode, 2)) Editor.ChangeEditMode(EditMode.Scale);

                    Text("Current edit axis:");
                    int editAxis = (int)Editor.CurrentEditAxis;
                    if (RadioButton("All", ref editAxis, 3)) Editor.ChangeEditAxis(EditAxis.All);
                    SameLine();
                    if (RadioButton("X", ref editAxis, 0)) Editor.ChangeEditAxis(EditAxis.X);
                    SameLine();
                    if (RadioButton("Y", ref editAxis, 1)) Editor.ChangeEditAxis(EditAxis.Y);
                    SameLine();
                    if (RadioButton("Z", ref editAxis, 2)) Editor.ChangeEditAxis(EditAxis.Z);

                    Text("Rotation mode:");
                    int pivotMode = Editor.SharedPivotRotationMode ? 1 : 0;
                    if (RadioButton("Shared pivot", ref pivotMode, 1)) Editor.SharedPivotRotationMode = true;
                    SameLine();
                    if (RadioButton("Unique pivot", ref pivotMode, 0)) Editor.SharedPivotRotationMode = false;

                    Text("Alignment");
                    float gridAlignment = Global.Config.GridAlignment;
                    if (InputFloat("GridAlignment", ref gridAlignment))
                    {
                        Global.Config.GridAlignment = Math.Abs(gridAlignment);
                        Editor.UpdateAlignmentValues();
                    }
                    float angleAlignment = Global.Config.AngleAlignment;
                    if (InputFloat("AngleAlignment", ref angleAlignment))
                    {
                        Global.Config.AngleAlignment = Math.Abs(angleAlignment);
                        Editor.UpdateAlignmentValues();
                    }
                    float scaleAlignment = Global.Config.ScaleAlignment;
                    if (InputFloat("ScaleAlignment", ref scaleAlignment))
                    {
                        Global.Config.ScaleAlignment = Math.Abs(scaleAlignment);
                        Editor.UpdateAlignmentValues();
                    }
                }

                if (CollapsingHeader("Scene overview", true))
                {
                    foreach (var entity in Global.EntityManager.Entities)
                    {
                        if (entity.Destroyed || entity.ContainsFlag(EntityFlag.EditorNotEditable))
                        {
                            continue;
                        }

                        bool selectedEntity = Editor.SelectedEntities.Contains(entity);
                        if (selectedEntity)
                        {
                            PushStyleColor(RyneImGuiColor.ImGuiCol_Text, new Float4(1, 1, 0, 1));
                        }

                        if (MenuItem(entity.Name))
                        {
                            bool addToSelection = Global.Application.KeyDown((int)RyneKey.LeftControl) || Global.Application.KeyDown((int)RyneKey.RightControl);
                            Editor.SelectEntity(entity, addToSelection);
                        }

                        if (selectedEntity)
                        {
                            PopStyleColor();
                        }
                    }
                }
            }
            End();
        }

        private void DrawLog()
        {
            if (Begin("Log"))
            {
                PushFont(GetFont("Consolas12"));

                var begin = Math.Max(0, Logger.CurrentLog.Count - 100);
                for (int i = begin; i < Logger.CurrentLog.Count; i++)
                {
                    var log = Logger.CurrentLog[i];
                    var color = Logger.GetColor(log.Item1);
                    PushStyleColor(RyneImGuiColor.ImGuiCol_Text, color);
                    Text(log.Item2);
                    PopStyleColor();
                }

                PopFont();

                SetScrollHereY(1.0f);
            }
            End();
        }

        private void DrawPropertiesBar()
        {
            if (Begin("Properties"))
            {
                if (Editor.SelectedEntity != null)
                {
                    Editor.SelectedEntity.RenderGui(this);

                    if (CollapsingHeader("Entity actions", true))
                    {
                        if (MenuItem("Rename"))
                        {
                            Editor.TryRenameEntity();
                        }
                        if (MenuItem("Delete"))
                        {
                            Editor.DeleteActiveEntities();
                        }
                    }
                }
            }
            End();
        }

        // All windows controlled by bools inside this class
        private unsafe void DrawExtensionWindows()
        {
            if (RenderSettingsToggle)
            {
                Begin("Change render settings", ref RenderSettingsToggle);

                int renderMode = SceneData.GetRenderMode();
                var values = Enum.GetNames(typeof(RyneRenderMode)).ToList();
                if (InputCombo("RenderMode", values, ref renderMode)) SceneData.ChangeRendermode((RyneRenderMode)renderMode);

                bool nee = SceneData.GetNextEventEstimation();
                if (InputCheckBox("Next event estimation", ref nee)) SceneData.SetNextEventEstimation(nee);

                bool beamRays = SceneData.GetUseBeamRays();
                if (InputCheckBox("Use Beam rays", ref beamRays)) SceneData.SetUseBeamRays(beamRays);

                int maxPathDepth = SceneData.GetMaxPathDepth();
                if (InputInt("Max path depth", ref maxPathDepth)) SceneData.SetMaxPathDepth((uint)(maxPathDepth % 8));

                int maxTransmissivePathDepth = SceneData.GetMaxTransmissivePathDepth();
                if (InputInt("Max transmissive path depth", ref maxTransmissivePathDepth)) SceneData.SetMaxTransmissivePathDepth((uint)(maxTransmissivePathDepth % 8));

                int maxSecondaryRayLevel = SceneData.GetMaxSecondaryRayLevel();
                if (InputInt("Max secondary ray voxel level", ref maxSecondaryRayLevel)) SceneData.SetMaxSecondaryRayLevel((uint)(maxSecondaryRayLevel % 20));

                End();
            }

            if (FpsWindowToggle)
            {
                var frameTimes = Global.EngineCore.FrameTimes.ToArray();
                float average = frameTimes.Sum() / frameTimes.Length;
                float averageFps = 1000.0f / average;
                Begin("FPS", ref FpsWindowToggle);
                Text($"Average Fps: {averageFps:0}");
                Text($"Average frametime: {average:0.00} ms");
                fixed (float* values = &frameTimes[0])
                {
                    PlotLines("Milliseconds", values, frameTimes.Length, 0, 32.0f, new Float2(200, 100));
                }

                End();
            }
        }

        private void RenderGenericWindows()
        {
            List<WindowGui> toRemove = new List<WindowGui>();

            foreach (var window in ActiveWindows)
            {
                window.BeginWindow();
                window.RenderContents();
                window.EndWindow();

                if (!window.Active)
                {
                    toRemove.Add(window);
                }
            }

            foreach (var window in toRemove)
            {
                ActiveWindows.Remove(window);
                window.OnClose();
            }

            foreach (var window in WindowsToAdd)
            {
                ActiveWindows.Add(window);
            }
            WindowsToAdd.Clear();
        }

        private void RenderPopups()
        {
            List<PopupGui> toRemove = new List<PopupGui>();

            foreach (var popup in ActivePopups)
            {
                if (!popup.PopupActive)
                {
                    OpenPopup(popup.WindowTitle);
                }

                popup.BeginWindow();
                popup.RenderContents();
                popup.EndWindow();

                if (!popup.Active)
                {
                    toRemove.Add(popup);
                }
            }

            foreach (var window in toRemove)
            {
                ActivePopups.Remove(window);
                window.OnClose();
            }
        }

        private void AddEntityWindow()
        {
            WindowGui window = new WindowGui(this, "Add entity");

            // Use reflection to find all entities
            foreach (var entityType in ReflectionCode.EntityTypes)
            {
                window.AddElement(new ButtonGuiElement(entityType.Name, result =>
                {
                    window.Active = false;
                    Entity e = (Entity)Activator.CreateInstance(entityType);
                    e.Name = Helpers.GetUniqueName(e.GetType().Name);
                    Editor.AddEntity(e);
                }));
            }

            AddWindow(window);
        }

        private int GetCurrentRenderCount()
        {
            if (!Directory.Exists("Renders"))
            {
                return 0;
            }

            int maxCount = 0;
            foreach (var file in Directory.GetFiles("Renders"))
            {
                var name = new FileInfo(file).Name;
                if (name.StartsWith("Render"))
                {
                    var withoutExtension = name.Split('.').First();
                    int count = int.Parse(withoutExtension.Substring(6, withoutExtension.Length - 6));
                    if (count > maxCount)
                        maxCount = count;
                }
            }

            return maxCount;
        }
    }
}
