using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Data;
using NodeGraph.Model;
using NodeGraph.ViewModel;

namespace NodeGraph
{
    class NodeGraphManager
    {
        #region Fields

        public static Dictionary<Guid, Flowchart> Flowcharts = new Dictionary<Guid, Flowchart>();
        public static Dictionary<Guid, Node> Nodes = new Dictionary<Guid, Node>();
        public static Dictionary<Guid, Port> Ports = new Dictionary<Guid, Port>();
        public static Dictionary<Guid, Connector> Connectors = new Dictionary<Guid, Connector>();

        #endregion

        #region Node

        public Node CreateNode(string header, Guid guid, Flowchart flowchart, Type type, double x, double y)
        {
            // Create Node
            Node node = Activator.CreateInstance(type, new object[] { guid, flowchart }) as Node;
            node.X = x;
            node.Y = y;
            node.Header = header;

            // Create ViewModel
            node.ViewModel = new NodeViewModel(node);

            // Register Node
            flowchart.Nodes.Add(node);
            Nodes.Add(guid, node);

            // Create Ports from Properties
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                PortAttribute[] portAttributes = propertyInfo.GetCustomAttributes(typeof(PortAttribute), false) as PortAttribute[];
                if (portAttributes != null)
                {
                    foreach (var attribute in portAttributes)
                    {
                        CreatePort(attribute.Name, Guid.NewGuid(), node, propertyInfo.PropertyType, attribute.IsInput);
                    }
                }
            }

            // Create Ports from Fields
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                PortAttribute[] portAttributes = fieldInfo.GetCustomAttributes(typeof(PortAttribute), false) as PortAttribute[];
                if (portAttributes != null)
                {
                    foreach (var attribute in portAttributes)
                    {
                        CreatePort(attribute.Name, Guid.NewGuid(), node, fieldInfo.FieldType, attribute.IsInput);
                    }
                }
            }

            return node;
        }

        public void DestroyNode(Guid guid)
        {
            if (Nodes.TryGetValue(guid, out Node node))
            {
                List<Guid> portGuids = new List<Guid>();

                foreach (var port in node.InputPorts)
                    portGuids.Add(port.Guid);

                foreach (var port in node.OutputPorts)
                    portGuids.Add(port.Guid);

                Nodes.Remove(guid);
            }
        }

        #endregion

        #region Port

        public Port CreatePort(string name, Guid guid, Node node, Type valueType, bool isInput)
        {
            // Create Port
            Port port = new Port(guid, node, isInput);
            port.Name = name;

            // Create ViewModel
            port.ViewModel = new PortViewModel(port);

            // Register Port
            if (port.IsInput)
            {
                node.InputPorts.Add(port);
                node.ViewModel.InputPortViewModels.Add(port.ViewModel);
            }
            else
            {
                node.OutputPorts.Add(port);
                node.ViewModel.OutputPortViewModels.Add(port.ViewModel);
            }
            Ports.Add(guid, port);

            return port;
        }

        public void DestroyPort(Guid guid)
        {
            if (Ports.TryGetValue(guid, out Port port))
            {

            }
        }

        #endregion
    }
}
