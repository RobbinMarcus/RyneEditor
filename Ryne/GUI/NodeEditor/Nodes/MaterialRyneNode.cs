using Ryne.Scene;

namespace Ryne.Gui.NodeEditor.Nodes
{
    class MaterialRyneNode : EditorNode
    {
        public Material Material { get; private set; }
        private readonly MaterialGui MaterialProperties;

        public MaterialRyneNode(Material material, MaterialNodeEditorGui editorGui) : base(editorGui)
        {
            Material = material;
            MaterialProperties = new MaterialGui(EditorGui.Gui, material, OnMaterialPropertiesChanged);
            Name = "Material";

            // Add all material inputs
            foreach (var field in typeof(Material).GetFields())
            {
                if (field.FieldType == typeof(float))
                {
                    AddInput(new RyneConnection { Name = field.Name, Type = RyneConnectionType.ConnectionType_Float });
                }

                //if (field.FieldType == typeof(Float3))
                //{
                //    AddInput(new RyneConnection { Name = field.Name, Type = RyneConnectionType.ConnectionType_Float3});
                //}
            }

            // Add input for normal
            //AddInput(new RyneConnection { Name = "Normal", Type = RyneConnectionType.ConnectionType_Float3 });
        }

        public override void DrawProperties()
        {
            base.DrawProperties();
            MaterialProperties.RenderContents();
        }

        // Function to chain material updates from the MaterialGui so we always have the latest material
        private void OnMaterialPropertiesChanged(WindowGui gui)
        {
            // Update material, but keep textures from the NodeEditor
            Material material = (Material) gui.GetWindowObject;
            material.Textures = Material.Textures;
            Material = material;

            EditorGui.OnMaterialChanged(gui);
        }
    }
}
