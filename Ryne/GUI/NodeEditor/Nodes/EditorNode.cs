namespace Ryne.Gui.NodeEditor.Nodes
{
    class EditorNode : RyneNode
    {
        protected MaterialNodeEditorGui EditorGui;

        public EditorNode(MaterialNodeEditorGui editorGui)
        {
            EditorGui = editorGui;
        }

        public virtual void OnRemove()
        {

        }

        public virtual void DrawProperties()
        {

        }

        public virtual void Draw()
        {

        }
    }
}
