using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using NodeGraph.Model;
using NodeGraph.ViewModel;

namespace NodeGraph
{
    // This project is based on the NodeGraph project by lifeisforu: https://github.com/lifeisforu/NodeGraph

    class NodeGraphManager
    {
        #region Fields

        public static Dictionary<Guid, Flowchart> Flowcharts = new Dictionary<Guid, Flowchart>();
        public static Dictionary<Guid, Node> Nodes = new Dictionary<Guid, Node>();
        public static Dictionary<Guid, Port> Ports = new Dictionary<Guid, Port>();
        public static Dictionary<Guid, Connector> Connectors = new Dictionary<Guid, Connector>();
        public static Dictionary<Guid, ObservableCollection<Guid>> SelectedNodes = new Dictionary<Guid, ObservableCollection<Guid>>();

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

                foreach (var portGuid in portGuids)
                {
                    DestroyPort(portGuid);
                }

                Flowchart flowchart = node.Owner;
                flowchart.Nodes.Remove(node);

                // TODO: Add history

                Nodes.Remove(guid);
            }
        }

        public static Node FindNode(Guid guid)
        {
            Nodes.TryGetValue(guid, out Node node);
            return node;
        }

        #endregion

        #region Port

        public static Port CreatePort(string name, Guid guid, Node node, Type valueType, bool isInput)
        {
            // Create Port
            Port port = new Port(guid, node, isInput, valueType);
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

            // TODO: Add History

            return port;
        }

        public static void DestroyPort(Guid guid)
        {
            if (Ports.TryGetValue(guid, out Port port))
            {

            }
        }

        public static Port FindPort(Guid guid)
        {
            Ports.TryGetValue(guid, out Port port);
            return port;
        }

        #endregion

        #region Connector

        public static bool IsConnecting { get; private set; }
        public static Port CurrentPort { get; private set; }
        public static Connector CurrentConnector { get; private set; }

        public static Connector CreateConnector(Guid guid, Flowchart flowchart)
        {
            Connector connector = new Connector(guid, flowchart);

            Connectors.Add(connector.Guid, connector);

            connector.ViewModel = new ConnectorViewModel(connector);
            flowchart.Connectors.Add(connector);

            return connector;
        }

        public static void DestroyConnector(Guid guid)
        {
            if (Connectors.TryGetValue(guid, out Connector connector))
            {
                if (connector.StartPort != null)
                {
                    DisconnectFrom(connector.StartPort, connector);
                }

                if (connector.EndPort != null)
                {
                    DisconnectFrom(connector.EndPort, connector);
                }

                Flowchart flowchart = connector.Owner;
                flowchart.Connectors.Remove(connector);

                Connectors.Remove(guid);
            }
        }

        public static void ConnectTo(Port port, Connector connector)
        {
            if (port.IsInput)
            {
                connector.EndPort = port;
            }
            else
            {
                connector.StartPort = port;
            }
            port.Connectors.Add(connector);
        }

        public static void DisconnectFrom(Port port, Connector connector)
        {
            if (port.IsInput)
            {
                connector.EndPort = null;
            }
            else
            {
                connector.StartPort = null;
            }
            port.Connectors.Remove(connector);
        }

        public static void DisconnectAll(Port port)
        {
            List<Guid> connectorGuids = new List<Guid>();
            foreach(var connector in port.Connectors)
            {
                connectorGuids.Add(connector.Guid);
            }

            foreach (var guid in connectorGuids)
            {
                DestroyConnector(guid);
            }
        }

        public static void BeginConnection(Port port)
        {
            IsConnecting = true;

            Node node = port.Owner;
            Flowchart flowchart = node.Owner;

            // TODO: Begin Drag

            CurrentConnector = CreateConnector(Guid.NewGuid(), flowchart);
            ConnectTo(port, CurrentConnector);

            CurrentPort = port;
        }

        public static void SetOtherConnectorPort(Port port)
        {
            if (port != null)
            {
                ConnectTo(port, CurrentConnector);
            }
            else
            {
                if (CurrentConnector.StartPort != null && CurrentConnector.EndPort == CurrentPort)
                {
                    DisconnectFrom(CurrentConnector.StartPort, CurrentConnector);
                }
                else if (CurrentConnector.EndPort != null && CurrentConnector.StartPort == CurrentPort)
                {
                    DisconnectFrom(CurrentConnector.EndPort, CurrentConnector);
                }
            }
        }

        // Declare once for better performance
        static List<Node> CheckedNodes = new List<Node>();

        public static bool CanConnect(Port otherPort)
        {
            // Same port check
            if (CurrentPort == otherPort)
                return false;

            // Type check
            Type firstType = CurrentPort.ValueType;
            Type otherType = otherPort.ValueType;

            if (firstType != otherType)
                return false; // TODO: Automatic type casting

            // Same node check
            Node firstNode = CurrentPort.Owner;
            Node otherNode = otherPort.Owner;

            if (firstNode == otherNode)
                return false;

            // Input<->Input or Output<->Output check
            if (CurrentPort.IsInput == otherPort.IsInput)
                return false;

            // Already connected check
            foreach (var connector in CurrentPort.Connectors)
            {
                if (connector.StartPort == otherPort || connector.EndPort == otherPort)
                    return false;
            }

            // Test for circular connections
            if (IsReachable(CurrentPort.IsInput ? firstNode : otherNode, CurrentPort.IsInput ? otherNode : firstNode))
            {
                return false;
            }

            return true;
        }

        private static bool IsReachable(Node from, Node to)
        {
            CheckedNodes.Clear();
            return IsReachableRecurse(from, to);
        }

        private static bool IsReachableRecurse(Node from, Node to)
        {
            if (CheckedNodes.Contains(from))
                return false;

            CheckedNodes.Add(from);

            foreach (var port in from.OutputPorts)
            {
                foreach (var connector in port.Connectors)
                {
                    Port endPort = connector.EndPort;
                    if (endPort.Owner == to)
                        return true;

                    if (IsReachable(endPort.Owner, to))
                        return true;
                }
            }

            return false;
        }

        public static void EndConnection(Port endPort = null)
        {
            // TODO: End dragging

            if (!IsConnecting)
            {
                return;
            }

            if (endPort != null)
            {
                SetOtherConnectorPort(endPort);
            }

            if (CurrentConnector.StartPort == null || CurrentConnector.EndPort == null)
            {
                DestroyConnector(CurrentConnector.Guid);
            }
            else
            {
                // TODO: Deal with single/multiple input stuff with destroying other connections
            }

            IsConnecting = false;
            CurrentConnector = null;
            CurrentPort = null;
        }

        public static void UpdateConnection(Point mousePosition)
        {
            // TODO: Update Connector Curve Data
        }

        #endregion

        #region Node Dragging

        public static bool IsNodeDragged { get; private set; }
        private static Guid _NodeDraggedFlowchartGuid;

        public static void BeginDragNode(Flowchart flowchart)
        {
            // TODO: Begin Drag

            IsNodeDragged = true;
            _NodeDraggedFlowchartGuid = flowchart.Guid;
        }

        public static void EndDragNode()
        {
            // TODO: End Drag

            IsNodeDragged = false;
        }

        public static void DragNode(Point delta)
        {
            if (!IsNodeDragged)
                return;

            if (SelectedNodes.TryGetValue(_NodeDraggedFlowchartGuid, out ObservableCollection<Guid> selectedNodes))
            {
                foreach (var guid in selectedNodes)
                {
                    Node node = FindNode(guid);
                    node.X += delta.X;
                    node.Y += delta.Y;
                }
            }
        }

        #endregion

        #region Mouse Trapping

        [DllImport("user32.dll")]
        public static extern void ClipCursor(ref System.Drawing.Rectangle rect);

        [DllImport("user32.dll")]
        public static extern void ClipCursor(IntPtr rect);

        public static bool IsDragging = false;

        public static void BeginDragging()
        {
            IsDragging = true;
        }

        #endregion
    }
}
