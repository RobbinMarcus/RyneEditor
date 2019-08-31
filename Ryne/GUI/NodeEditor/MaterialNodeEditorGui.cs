using Ryne.Gui.NodeEditor.Nodes;
using Ryne.Scene;

namespace Ryne.Gui.NodeEditor
{
    // TODO: this NodeEditor is pretty experimental, and there are some issues when adding new textures. 
    class MaterialNodeEditorGui : NodeEditorGui
    {
        public Material Material { get; set; }
        public WindowDelegate OnMaterialChanged { get; }

        public override object GetWindowObject => Material;

        private readonly RyneDelegate OnInputUpdatedEvent;
        private readonly RyneDelegate OnOutputUpdatedEvent;
        private readonly RyneDelegate OnConnectionRemovedEvent;

        private readonly RyneDelegate AddTextureEvent;

        private readonly MaterialRyneNode MaterialNode;

        public MaterialNodeEditorGui(ImGuiWrapper gui, Material material, WindowDelegate onMaterialChanged) : base(gui, "Material editor: " + material.Name)
        {
            Material = material;
            OnMaterialChanged = onMaterialChanged;

            AddTextureEvent = new RyneDelegate(AddTexture);
            NodeEditor.AddRightClickAction("Add texture", AddTextureEvent);

            MinWidth = 800.0f;
            MinHeight = 600.0f;

            MaterialNode = new MaterialRyneNode(Material, this);
            OnInputUpdatedEvent = new RyneDelegate(OnMaterialInputUpdated);
            OnOutputUpdatedEvent = new RyneDelegate(OnOutputToMaterialUpdated);
            OnConnectionRemovedEvent = new RyneDelegate(OnMaterialInputRemoved);

            MaterialNode.SetOnInputSetEvent(OnInputUpdatedEvent);
            MaterialNode.SetOnOutputSetEvent(OnOutputUpdatedEvent);
            MaterialNode.SetOnInputConnectionRemovedEvent(OnConnectionRemovedEvent);

            // Add nodes
            int materialNodeId = NodeEditor.AddNode(MaterialNode, new Float2(450.0f, 300.0f));
            for (int i = 0; i < Material.Textures.Count; i++)
            {
                var texture = Material.Textures[i];
                int nodeId = NodeEditor.AddNode(new TextureRyneNode(texture, this), new Float2(150.0f, 300.0f + i * 100.0f));

                switch (texture.Type)
                {
                    case RyneTextureType.MappedChannelsTexture:
                        if (texture.RChannelOffset > -1) NodeEditor.AddConnection(nodeId, 0, materialNodeId, texture.RChannelOffset);
                        if (texture.GChannelOffset > -1) NodeEditor.AddConnection(nodeId, 1, materialNodeId, texture.GChannelOffset);
                        if (texture.BChannelOffset > -1) NodeEditor.AddConnection(nodeId, 2, materialNodeId, texture.BChannelOffset);
                        break;
                }
            }

            NodeEditor.SelectNode(materialNodeId);
        }

        public override void OnClose()
        {
            base.OnClose();
            OnInputUpdatedEvent.Free();
            OnOutputUpdatedEvent.Free();
            OnConnectionRemovedEvent.Free();
            AddTextureEvent.Free();
        }

        private void OnMaterialInputUpdated(int inputId, int outputId, int nodeId)
        {
            RyneNode outputNode = NodeEditor.Nodes[nodeId];
            if (outputNode is TextureRyneNode textureNode)
            {
                int materialTextureId = Material.GetTextureListId(textureNode.Texture);
                UpdateTexture(ref textureNode.Texture, inputId, outputId);
                Material.Textures[materialTextureId] = textureNode.Texture;
            }

            OnMaterialChanged?.Invoke(this);
        }

        private void OnOutputToMaterialUpdated(int inputId, int outputId, int nodeId)
        {
            RyneNode outputNode = NodeEditor.Nodes[nodeId];
            if (outputNode is TextureRyneNode textureNode)
            {
                int materialTextureId = Material.GetTextureListId(textureNode.Texture);
                UpdateTexture(ref textureNode.Texture, inputId, outputId);
                Material.Textures[materialTextureId] = textureNode.Texture;
            }

            OnMaterialChanged?.Invoke(this);
        }

        private void OnMaterialInputRemoved(int inputId, int outputId, int nodeId)
        {
            RyneNode outputNode = NodeEditor.Nodes[nodeId];
            if (outputNode is TextureRyneNode textureNode)
            {
                //int materialTextureId = Material.GetTextureListId(textureNode.Texture.Filename);
                //var textureInstance = Material.Textures[materialTextureId];
                //UpdateTexture(ref textureInstance, inputId, outputId);
                //Material.Textures[materialTextureId] = textureInstance;
            }

            OnMaterialChanged?.Invoke(this);
        }


        private void UpdateTexture(ref RyneTextureInstance texture, int inputId, int outputId)
        {
            switch (texture.Type)
            {
                case RyneTextureType.AlbedoTexture:
                    if (inputId == GetMaterialFieldId("Normal"))
                    {
                        texture.Type = RyneTextureType.NormalMapTexture;
                    }
                    break;

                case RyneTextureType.NormalMapTexture:
                    if (inputId == GetMaterialFieldId("Albedo"))
                    {
                        texture.Type = RyneTextureType.NormalMapTexture;
                    }
                    break;

                case RyneTextureType.MappedChannelsTexture:
                    switch (outputId)
                    {
                        case 0:
                            if (texture.RChannelOffset > -1) NodeEditor.RemoveConnection(MaterialNode.NodeId, texture.RChannelOffset);
                            texture.RChannelOffset = (sbyte)inputId;
                            break;
                        case 1:
                            if (texture.GChannelOffset > -1) NodeEditor.RemoveConnection(MaterialNode.NodeId, texture.GChannelOffset);
                            texture.GChannelOffset = (sbyte)inputId;
                            break;
                        case 2:
                            if (texture.BChannelOffset > -1) NodeEditor.RemoveConnection(MaterialNode.NodeId, texture.BChannelOffset);
                            texture.BChannelOffset = (sbyte)inputId;
                            break;
                    }
                    break;
            }
        }


        private void AddTexture()
        {
            NodeEditor.AddNodeOnCursorPosition(new TextureRyneNode(null, this));
        }

        private int GetMaterialFieldId(string fieldName)
        {
            var fields = typeof(Material).GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name == fieldName)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
