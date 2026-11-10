using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Editor.BehaviorTreeEditor
{
    public class NodeView : Node
    {
        public BtBaseNode nodeData;
        public Port input;
        public Port output;

        public NodeView(BtBaseNode _nodeData)
        {
            nodeData = _nodeData;
            title = _nodeData.name;
            viewDataKey = _nodeData.Guid;
            
            SetPosition(new Rect(_nodeData.position,Vector2.zero));
            
            CreateInputPorts();
            CreateOutputPorts();
        }

        private void CreateInputPorts()
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            if (input != null)
            {
                input.portName = "In";
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts()
        {
            if (nodeData is ActionNode)
            {
            }
            else if (nodeData is CompositeNode)
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            else if (nodeData is DecoratorNode)
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));

            if (output != null)
            {
                output.portName = "Out";
                outputContainer.Add(output);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            nodeData.position.x = newPos.x;
            nodeData.position.y = newPos.y;

#if UNITY_EDITOR
            // 标记资产已被修改，触发保存
            EditorUtility.SetDirty(nodeData);
#endif

        }
    }
}