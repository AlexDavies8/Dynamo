using Dynamo.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo
{
    static class GlobalState
    {
        public static void Initialise()
        {
            NodeGraph.NodeGraphManager.OnSelectionChanged += node => ActiveNode = node;
        }

        private static NodeGraph.Model.Node _activeNode;
        public static NodeGraph.Model.Node ActiveNode
        {
            get => _activeNode;
            set
            {
                if (_activeNode != value)
                {
                    _activeNode = value;
                    ActiveNodeChanged(_activeNode);
                }
            }
        }
        public static Action<NodeGraph.Model.Node> ActiveNodeChanged { get; set; }
    }
}
