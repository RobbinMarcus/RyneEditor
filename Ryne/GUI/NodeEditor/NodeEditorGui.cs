using Ryne.Utility;

namespace Ryne.Gui.NodeEditor
{
    class NodeEditorGui : WindowGui
    {
        public NodeEditor NodeEditor;

        public NodeEditorGui(ImGuiWrapper gui, string windowTitle) : base(gui, windowTitle)
        {
            NodeEditor = new NodeEditor();
            MinWidth = 500.0f;
            MinHeight = 500.0f;

            NodeEditor.SetOnNodeRemovedEvent(OnNodeRemoved);
        }

        public override void RenderContents()
        {
            if (Active)
            {
                NodeEditor.Draw();
            }
        }

        protected void OnNodeRemoved(int nodeId)
        {
        }
    }
}
