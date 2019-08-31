using System.Collections.Generic;
using System.IO;
using Ryne.Utility;

namespace Ryne.Gui.NodeEditor.Nodes
{
    class TextureRyneNode : EditorNode
    {
        public RyneTextureInstance Texture;

        public TextureRyneNode(RyneTextureInstance texture, MaterialNodeEditorGui editorGui) : base(editorGui)
        {
            Texture = texture;
            Name = "Texture";

            if (Texture != null)
            {
                SetTextureType(Texture.Type);
            }
        }

        public void SetTextureType(RyneTextureType type)
        {
            Texture.Type = type;
            UpdateOutputs();
        }

        public override void OnRemove()
        {
            base.OnRemove();

            if (Texture != null)
            {
                EditorGui.Material.Textures.Remove(Texture);
                EditorGui.OnMaterialChanged(EditorGui);
            }
        }

        public override void DrawProperties()
        {
            base.DrawProperties();

            if (EditorGui.Gui.MenuItem("Load texture"))
            {
                var extensions = Global.ResourceManager.GetSupportedExtensions(RyneResourceType.ResourceTypeTexture);
                var window = new FileExplorerGui(EditorGui.Gui, "Textures", extensions, "Select texture");
                window.Callback += result =>
                {
                    if (Texture != null)
                    {
                        EditorGui.Material.Textures.Remove(Texture);
                    }

                    var file = ((FileExplorerGui)result).SelectedFile;
                    Global.ResourceManager.GetResourceIndex(file, RyneResourceType.ResourceTypeTexture, true);
                    Texture = new RyneTextureInstance
                    {
                        Filename = file
                    };
                    SetTextureType(RyneTextureType.AlbedoTexture);
                    EditorGui.Material.Textures.Add(Texture);
                    EditorGui.OnMaterialChanged(EditorGui);
                };
                ((SceneEditorGui)EditorGui.Gui).AddPopup(window);
            }

            if (Texture != null)
            {
                EditorGui.Gui.Text("Texture: " + new FileInfo(Texture.Filename).Name);

                int selected = (int) Texture.Type;
                if (EditorGui.Gui.InputCombo("Type", new List<string> {"Albedo", "Normal", "Channels"}, ref selected))
                {
                    SetTextureType((RyneTextureType)selected);
                    EditorGui.OnMaterialChanged(EditorGui);
                }

                EditorGui.Gui.RenderTexture(Texture, new Float2(100, 100));
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

        private void UpdateOutputs()
        {
            ClearOutputs();

            switch (Texture.Type)
            {
                case RyneTextureType.AlbedoTexture:
                case RyneTextureType.NormalMapTexture:
                    AddOutput(new RyneConnection { Name = "TextureData", Type = RyneConnectionType.ConnectionType_Float3 });
                    break;
                case RyneTextureType.MappedChannelsTexture:
                    AddOutput(new RyneConnection { Name = "R", Type = RyneConnectionType.ConnectionType_Float });
                    AddOutput(new RyneConnection { Name = "G", Type = RyneConnectionType.ConnectionType_Float });
                    AddOutput(new RyneConnection { Name = "B", Type = RyneConnectionType.ConnectionType_Float });
                    break;
            }

            if (NodeId > -1)
            {
                EditorGui.NodeEditor.UpdateNode(this, NodeId);
            }
        }
    }
}
