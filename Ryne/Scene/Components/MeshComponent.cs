using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Ryne.Entities;
using Ryne.Gui;
using Ryne.Utility;
using Ryne.Utility.Math;
using MessagePack;
using Ryne.Gui.Windows;

namespace Ryne.Scene.Components
{
    public enum MeshFlag
    {
        Loaded,         // Mesh done loading
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MeshComponent
    {
        [IgnoreMember]
        public int MeshFlags { get; private set; }

        [IgnoreMember]
        public RyneNotifyFunction OnMeshLoadedCallback { get; set; }

        public RyneObjectType ObjectType { get; private set; }

        public string MeshFilename { get; private set; }

        // Identifier for model index in rendering backend
        [IgnoreMember]
        public int GeometryIndex { get; private set; }

        public List<Material> CustomMaterials { get; private set; }

        public void RenderGui(ImGuiWrapper gui, Entity owner)
        {
            if (!(gui is SceneEditorGui sceneGui))
            {
                return;
            }

            if (gui.CollapsingHeader("Mesh component", true))
            {
                if (ObjectType == RyneObjectType.ObjectTypeNone)
                {
                    if (sceneGui.MenuItem("Load voxel mesh"))
                    {
                        var extensions = Global.ResourceManager.GetSupportedExtensions(RyneResourceType.ResourceTypeBsvDag);
                        var window = new FileExplorerGui(sceneGui, "VoxelModels", extensions, "Select model");
                        window.Callback += result =>
                        {
                            var fileExplorer = (FileExplorerGui)result;
                            var type = RyneObjectType.ObjectTypeBsvDag;
                            owner.Mesh.SetMeshData(fileExplorer.SelectedFile, type);
                            owner.Mesh.LoadMesh();
                            owner.SetChangedInEditor(true);
                        };
                        sceneGui.AddPopup(window);
                    }
                }
                else
                {
                    if (!Loaded || owner.RenderId == -1)
                    {
                        gui.Text("Mesh loaded: " + Loaded);
                        return;
                    }

                    if (CustomMaterials.Count == 0)
                    {
                        if (gui.MenuItem("Create unique materials"))
                        {
                            SetCustomMaterials(owner);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < CustomMaterials.Count; i++)
                        {
                            var localIndex = i;
                            var material = CustomMaterials[i];
                            var name = string.IsNullOrEmpty(material.Name) ? localIndex.ToString() : material.Name;
                            if (gui.MenuItem("Edit Material " + name))
                            {
                                var window = new MaterialGui(sceneGui, material, result =>
                                {
                                    owner.Mesh.UpdateMaterial(owner, sceneGui.SceneData, (Material)result.GetWindowObject, localIndex);
                                });
                                sceneGui.AddWindow(window);
                            }
                        }
                    }
                }
            }
        }

        public void PostDeserialize()
        {
            if (MeshDataSet())
            {
                // TODO: async?
                LoadMesh();
            }
        }

        public void SetMeshData(string filename, RyneObjectType type)
        {
            MeshFilename = filename;
            ObjectType = type;
        }

        public void LoadMesh()
        {
            if ((int)ObjectType != (int)RyneObjectType.ObjectTypeBsvDag)
            {
                Logger.Error($"Loading mesh: Wrong type: {ObjectType}");
                return;
            }

            if (!new FileInfo(Global.Config.WorkingDirectory + MeshFilename).Exists)
            {
                Logger.Error($"Loading mesh: {MeshFilename} doesn't exist");
                return;
            }

            // TODO: async
            Timer timer = new Timer();
            timer.Start();

            Logger.Log($"Loading mesh {MeshFilename}");
            GeometryIndex = (int)Global.ResourceManager.GetResourceIndex(MeshFilename, (RyneResourceType)(int)ObjectType, true);

            timer.Stop();
            Logger.Log(string.Format("ModelLoading: Loading {1} took: {0} ms", timer.MilliSeconds, MeshFilename));

            SetFlag(MeshFlag.Loaded, true);
            OnMeshLoadedCallback?.Invoke();
        }

        public void SetDefaults()
        {
            CustomMaterials = new List<Material>();
        }

        // Sets default only if items aren't set
        public void TrySetDefaults()
        {
            if (CustomMaterials is null)
            {
                CustomMaterials = new List<Material>();
            }
        }

        public void Destroy()
        {
            MeshFlags = 0;
            MeshFilename = "";
            GeometryIndex = 0;
            ObjectType = RyneObjectType.ObjectTypeNone;
            CustomMaterials = new List<Material>();
        }

        // Flags

        public bool ContainsFlag(MeshFlag flag)
        {
            return Bitmask.CheckBit(MeshFlags, (int)flag);
        }

        public void SetFlag(MeshFlag flag, bool value)
        {
            MeshFlags = Bitmask.SetBitTo(MeshFlags, (int)flag, value ? 1 : 0);
        }

        [IgnoreMember]
        public bool Loaded => ContainsFlag(MeshFlag.Loaded);


        private bool MeshDataSet()
        {
            bool meshDataSet = (int)ObjectType == (int)RyneObjectType.ObjectTypeBsvDag;
            meshDataSet &= !string.IsNullOrEmpty(MeshFilename);
            return meshDataSet;
        }

        public void SetCustomMaterials(Entity owner)
        {
            var sceneRenderData = Global.StateManager.GetCurrentState().SceneRenderData;
            sceneRenderData.CreateSeparateMaterialsForInstance(owner.RenderId);

            var materialCount = sceneRenderData.GetMaterialCount(owner.RenderId);
            for (var i = 0; i < materialCount; i++)
            {
                var material = sceneRenderData.GetMaterial(owner.RenderId, i);
                CustomMaterials.Add(Material.Create(material));
            }
        }

        public void OnRegistered(Entity owner)
        {
            if (owner.RenderId < 0)
            {
                Logger.Error("Mesh loaded but invalid RenderId");
                return;
            }

            // Set custom materials if we have any
            if (CustomMaterials.Count > 0)
            {
                var state = Global.StateManager.GetCurrentState();
                var materialCount = state.SceneRenderData.GetMaterialCount(owner.RenderId);
                if (materialCount != CustomMaterials.Count)
                {
                    Logger.Warning("Tried to set custom materials on entity but serialized material count isn't the same as active material count");
                }

                state.SceneRenderData.CreateSeparateMaterialsForInstance(owner.RenderId);
                for (var i = 0; i < CustomMaterials.Count; i++)
                {
                    state.SceneRenderData.SetMaterial(owner.RenderId, i, CustomMaterials[i].ToRenderMaterial());
                }
            }
        }


        public void UpdateMaterial(Entity owner, RyneSceneRenderData renderData, Material newMaterial, int materialId)
        {
            owner.SetChangedInEditor(true);
            // Update material in render backend
            renderData.SetMaterial(owner.RenderId, materialId, newMaterial.ToRenderMaterial());
            // Update local material
            if (owner.Mesh.CustomMaterials.Count > materialId)
            {
                owner.Mesh.CustomMaterials[materialId] = Material.Create(renderData.GetMaterial(owner.RenderId, materialId));
            }
        }
    }
}
