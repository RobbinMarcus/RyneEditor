using System.Linq;
using Ryne.Gui.NodeEditor.Nodes;
using Ryne.Utility;
using Ryne.Utility.Collections;

namespace Ryne.Gui.NodeEditor
{
    class NodeEditor : RyneNodeEditor
    {
        public DynamicArray<EditorNode> Nodes { get; private set; }

        private RyneDelegate BaseOnNodeRemovedEvent;
        private RyneNodeChangedFunction OnNodeRemovedEvent;

        public NodeEditor()
        {
            Nodes = new DynamicArray<EditorNode>();

            // Link OnNodeRemoved event to the backend
            BaseOnNodeRemovedEvent = new RyneDelegate(OnNodeRemoved);
            base.SetOnNodeRemovedEvent(BaseOnNodeRemovedEvent);
        }

        ~NodeEditor()
        {
            BaseOnNodeRemovedEvent.Free();
        }

        public int AddNode(EditorNode node, Float2 position)
        {
            Nodes.Add(node);
            int nodeId = base.AddNode(node, position);
            if (nodeId != Nodes.Length - 1)
            {
                Logger.Error("NodeEditor.AddNode: Invalid node id");
            }

            node.NodeId = nodeId;
            return nodeId;
        }

        public int AddNodeOnCursorPosition(EditorNode node)
        {
            int nodeId = base.AddNodeOnCursorPosition(node);
            Nodes.Add(node);
            if (nodeId != Nodes.Length - 1)
            {
                Logger.Error("NodeEditor.AddNodeOnCursorPosition: Invalid node id");
            }
            node.NodeId = nodeId;

            return nodeId;
        }

        public void Draw()
        {
            // Render node properties
            StartProperties();
            DrawProperties();
            EndProperties();

            StartDraw();

            // Render all nodes
            foreach (var node in Nodes)
            {
                StartNode(node.NodeId);
                node.Draw();
                EndNode();
            }

            EndDraw();
        }

        public void SetOnNodeRemovedEvent(RyneNodeChangedFunction function)
        {
            OnNodeRemovedEvent = function;
        }

        protected virtual void DrawProperties()
        {
            int selectedNodeId = GetSelectedNodeId();
            if (selectedNodeId > -1)
            {
                EditorNode node = Nodes.FirstOrDefault(x => x.NodeId == selectedNodeId);
                node?.DrawProperties();
            }
        }

        private void OnNodeRemoved(int nodeId)
        {
            OnNodeRemovedEvent?.Invoke(nodeId);

            Nodes.First(x => x.NodeId == nodeId).OnRemove();

            DynamicArray<EditorNode> newNodes = new DynamicArray<EditorNode>(0, Nodes.Length);

            foreach (var node in Nodes)
            {
                if (node.NodeId != nodeId)
                    newNodes.Add(node);
            }

            Nodes = newNodes;
        }
    }
}
