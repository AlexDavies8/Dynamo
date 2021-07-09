using System;
using System.Collections.Generic;
using System.Text;
using NodeGraph.Model;
using NodeGraph.ViewModel;

namespace NodeGraph
{
    class NodeGraphManager
    {

        #region Node

        public void CreateNode(string header, Guid guid, Flowchart flowchart, Type type, double x, double y)
        {
            NodeAttribute[] nodeAttributes = type.GetCustomAttributes(typeof(NodeAttribute), false) as NodeAttribute[];
            NodeAttribute nodeAttribute = nodeAttributes[0];

            Node node = Activator.CreateInstance(type, new object[] { guid, flowchart }) as Node;
            node.X = x;
            node.Y = y;
            node.Header = header;
        }

        #endregion

        #region Port

        public void CreatePort()
        {

        }

        #endregion
    }
}
