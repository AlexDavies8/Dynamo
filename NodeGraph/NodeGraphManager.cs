using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using NodeGraph.Model;
using NodeGraph.View;
using NodeGraph.ViewModel;

namespace NodeGraph
{
    // This project is based on the NodeGraph project by lifeisforu: https://github.com/lifeisforu/NodeGraph

    public class NodeGraphManager
    {
        #region Fields

        public static Dictionary<Guid, Flowchart> Flowcharts = new Dictionary<Guid, Flowchart>();
        public static Dictionary<Guid, Node> Nodes = new Dictionary<Guid, Node>();
        public static Dictionary<Guid, Port> Ports = new Dictionary<Guid, Port>();
        public static Dictionary<Guid, Connector> Connectors = new Dictionary<Guid, Connector>();
        public static Dictionary<Guid, ObservableCollection<Guid>> SelectedNodes = new Dictionary<Guid, ObservableCollection<Guid>>();

        #endregion

        #region Flowchart

        public static Flowchart CreateFlowchart(Guid guid)
        {
            Flowchart flowchart = new Flowchart();
            Flowcharts.Add(flowchart.Guid, flowchart);

            flowchart.ViewModel = new FlowchartViewModel(flowchart);

            ObservableCollection<Guid> selectionList = new ObservableCollection<Guid>();
            selectionList.CollectionChanged += NodeSelectionListCollectionChanged;
            SelectedNodes.Add(flowchart.Guid, selectionList);

            return flowchart;
        }

        public static void DestroyFlowchart(Guid guid)
        {
            if (!Flowcharts.TryGetValue(guid, out Flowchart flowchart))
                return;

            ObservableCollection<Guid> guids = new ObservableCollection<Guid>();
            foreach(var node in flowchart.Nodes)
            {
                guids.Add(node.Guid);
            }
            foreach(var nodeGuid in guids)
            {
                DestroyNode(nodeGuid);
            }

            SelectedNodes.Remove(guid);
            Flowcharts.Remove(guid);
        }

        public static Flowchart FindFlowchart(Guid guid)
        {
            Flowcharts.TryGetValue(guid, out Flowchart flowchart);
            return flowchart;
        }

        #endregion

        #region Node

        public static Node CreateNode(string header, Guid guid, Flowchart flowchart, Type type, double x, double y)
        {
            // Create Node
            Node node = Activator.CreateInstance(type, new object[] { guid, flowchart }) as Node;
            node.X = x;
            node.Y = y;
            node.Header = header;

            // Create ViewModel
            node.ViewModel = new NodeViewModel(node);
            flowchart.ViewModel.NodeViewModels.Add(node.ViewModel);

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
                        Port port = CreatePort(attribute.Name, Guid.NewGuid(), node, propertyInfo.PropertyType, attribute.IsInput, attribute.HasEditor, () => propertyInfo.GetValue(node));
                        port.PortValueChanged += (Port port, object prevValue, object newValue) =>
                        {
                            node.OnPortChanged?.Invoke(port);
                            propertyInfo.SetValue(node, newValue);
                        };
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
                        Port port = CreatePort(attribute.Name, Guid.NewGuid(), node, fieldInfo.FieldType, attribute.IsInput, attribute.HasEditor, () => fieldInfo.GetValue(node));
                        port.PortValueChanged += (Port port, object prevValue, object newValue) =>
                        {
                            fieldInfo.SetValue(node, newValue);
                        };
                    }
                }
            }

            node.OnCreate();

            return node;
        }

        public static void DestroyNode(Guid guid)
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

        public static Port CreatePort(string name, Guid guid, Node node, Type valueType, bool isInput, bool hasEditor, Func<object> getValue = null)
        {
            // Create Port
            Port port = new Port(guid, node, isInput, valueType, hasEditor, getValue);
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
            flowchart.ViewModel.ConnectorViewModels.Add(connector.ViewModel);

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
                flowchart.ViewModel.ConnectorViewModels.Remove(connector.ViewModel);

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

            BeginDragging(flowchart.ViewModel.View);

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

        public static bool CanConnect(Port otherPort, out string error)
        {
            error = "";

            // Same port check
            if (CurrentPort == otherPort)
            {
                error = "Cannot connect to the same port";
                return false;
            }

            // Type check
            Type firstType = CurrentPort.ValueType;
            Type otherType = otherPort.ValueType;

            if (firstType != otherType)
            {
                error = "Must connect to a port of the same type";
                return false; // TODO: Automatic type casting
            }

            // Same node check
            Node firstNode = CurrentPort.Owner;
            Node otherNode = otherPort.Owner;

            if (firstNode == otherNode)
            {
                error = "Cannot connect to the same Node";
                return false;
            }

            // Input<->Input or Output<->Output check
            if (CurrentPort.IsInput == otherPort.IsInput)
            {
                error = "An input port must be connected to an output port";
                return false;
            }

            // Already connected check
            foreach (var connector in CurrentPort.Connectors)
            {
                if (connector.StartPort == otherPort || connector.EndPort == otherPort)
                {
                    error = "Ports are already connected";
                    return false;
                }
            }

            // Test for circular connections
            if (IsReachable(CurrentPort.IsInput ? firstNode : otherNode, CurrentPort.IsInput ? otherNode : firstNode))
            {
                error = "Cannot create a cycle";
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
            EndDragging();

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
            if (CurrentConnector != null)
                CurrentConnector.ViewModel.View.BuildCurveData(mousePosition);
        }

        #endregion

        #region Node Dragging

        public static bool IsNodeDragged { get; private set; }
        private static Guid _NodeDraggedFlowchartGuid;

        public static void BeginDragNode(Flowchart flowchart)
        {
            BeginDragging(flowchart.ViewModel.View);

            IsNodeDragged = true;
            _NodeDraggedFlowchartGuid = flowchart.Guid;
        }

        public static void EndDragNode()
        {
            EndDragging();

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
        private static FlowchartView _trappingFlowchartView;

        public static void BeginDragging(FlowchartView flowchartView)
        {
            IsDragging = true;
            _trappingFlowchartView = flowchartView;

            PresentationSource source = PresentationSource.FromVisual(flowchartView);

            double scaleX = 1, scaleY = 1;
            if (source != null)
            {
                scaleX = source.CompositionTarget.TransformToDevice.M11;
                scaleY = source.CompositionTarget.TransformToDevice.M22;
            }

            Point startPosition = flowchartView.PointToScreen(new Point(0, 0));

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(
                (int)startPosition.X,
                (int)startPosition.Y,
                (int)(startPosition.X + flowchartView.ActualWidth * scaleX),
                (int)(startPosition.Y + flowchartView.ActualHeight * scaleY));

            ClipCursor(ref rect);

        }

        public static void EndDragging()
        {
            if (_trappingFlowchartView != null)
            {
                IsDragging = false;
                _trappingFlowchartView = null;
            }
            ClipCursor(IntPtr.Zero);
        }

        #endregion

        #region Node Selection

        public static Node ClickedNode { get; set; }

        public static ObservableCollection<Guid> GetSelectedNodeGuids(Flowchart flowchart)
        {
            if (SelectedNodes.TryGetValue(flowchart.Guid, out ObservableCollection<Guid> nodes))
                return nodes;
            return null;
        }

        public static void TrySelection(Flowchart flowchart, Node node)
        {
            // TODO: Add Selection Modes
            DeselectAllNodes(flowchart);

            if (!node.ViewModel.IsSelected)
            {
                SelectNode(node);

                // TODO: Implement History
            }
        }

        public static void SelectNode(Node node)
        {
            if (node.ViewModel.IsSelected)
            {
                return;
            }

            ObservableCollection<Guid> selected = GetSelectedNodeGuids(node.Owner);
            if (!selected.Contains(node.Guid))
            {
                node.ViewModel.IsSelected = true;
                selected.Add(node.Guid);
            }

            MoveNodeToFront(node);
        }

        public static void DeselectNode(Node node)
        {
            ObservableCollection<Guid> selected = GetSelectedNodeGuids(node.Owner);
            node.ViewModel.IsSelected = false;
            selected.Remove(node.Guid);
        }

        public static void DeselectAllNodes(Flowchart flowchart)
        {
            ObservableCollection<Guid> selected = GetSelectedNodeGuids(flowchart);

            foreach (var guid in selected)
            {
                Node node = FindNode(guid);
                node.ViewModel.IsSelected = false;
            }
            selected.Clear();
        }

        public static void SelectAllNodes(Flowchart flowchart)
        {
            DeselectAllNodes(flowchart);

            ObservableCollection<Guid> selected = GetSelectedNodeGuids(flowchart);
            foreach (var pair in Nodes)
            {
                Node node = pair.Value;
                if (node.Owner == flowchart)
                {
                    node.ViewModel.IsSelected = true;
                    selected.Add(node.Guid);
                }
            }
        }

        public static bool IsSelecting => _selectingFlowchart != null;
        private static Flowchart _selectingFlowchart;
        public static Point SelectStartPosition { get; private set; }

        public static void BeginSelection(Flowchart flowchart, Point start)
        {
            FlowchartView flowchartView = flowchart.ViewModel.View;
            BeginDragging(flowchartView);

            SelectStartPosition = start;

            _selectingFlowchart = flowchart;
            _selectingFlowchart.ViewModel.SelectionVisibility = Visibility.Visible;
        }

        public static void UpdateSelection(Flowchart flowchart, Point end)
        {
            FlowchartView flowchartView = flowchart.ViewModel.View;

            Point start = SelectStartPosition;

            Point selectionMin = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point selectionMax = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));
        
            foreach (var pair in Nodes)
            {
                Node node = pair.Value;
                if (node.Owner == _selectingFlowchart && node.ViewModel.View != null)
                {
                    Point nodeMin = new Point(node.X, node.Y);
                    Point nodeMax = new Point(
                        node.X + node.ViewModel.View.ActualWidth,
                        node.Y + node.ViewModel.View.ActualHeight
                    );

                    // TODO: Selection Modes (fully inside selection or partially)
                    bool isInside = (
                        nodeMin.X >= selectionMin.X && 
                        nodeMin.Y >= selectionMin.Y &&
                        nodeMax.X <= selectionMax.X &&
                        nodeMax.Y <= selectionMax.Y);

                    if (isInside)
                    {
                        SelectNode(node);
                    }
                }
            }
        }

        public static void EndSelection()
        {
            EndDragging();

            if (_selectingFlowchart == null)
                return;

            _selectingFlowchart.ViewModel.SelectionVisibility = Visibility.Collapsed;
            _selectingFlowchart = null;
        }

        #endregion

        #region Z Index

        public static void MoveNodeToFront(Node node)
        {
            int maxZIndex = int.MinValue;
            foreach (var pair in Nodes)
            {
                Node currentNode = pair.Value;
                maxZIndex = Math.Max(maxZIndex, currentNode.ZIndex);
                currentNode.ZIndex--;
            }

            node.ZIndex = maxZIndex;
        }

        #endregion

        #region Delete

        public static void DestroySelectedNodes(Flowchart flowchart)
        {
            ObservableCollection<Guid> selected = GetSelectedNodeGuids(flowchart);

            List<Guid> guids = new List<Guid>();
            foreach (var guid in selected)
            {
                guids.Add(guid);
            }

            foreach (var guid in guids)
            {
                DestroyNode(guid);
            }
        }

        #endregion

        #region ContentSize

        public static (double minX, double minY, double maxX, double maxY) CalculateContentBounds(Flowchart flowchart)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            bool found = false;

            foreach (var pair in Nodes)
            {
                Node node = pair.Value;
                NodeView nodeView = node.ViewModel.View;
                
                if (node.Owner == flowchart)
                {
                    minX = Math.Min(minX, node.X);
                    minY = Math.Min(minY, node.Y);
                    maxX = Math.Max(maxX, node.X + node.ViewModel.View.ActualWidth);
                    maxY = Math.Max(maxY, node.Y + node.ViewModel.View.ActualHeight);
                    found = true;
                }
            }

            if (!found)
            {
                minX = minY = maxX = maxY = 0;
            }

            return (minX, minY, maxX, maxY);
        }

        #endregion

        #region Selection Events

        public delegate void NodeSelectionChangedDelegate(Flowchart flowchart, ObservableCollection<Guid> nodes, NotifyCollectionChangedEventArgs args);
        public static event NodeSelectionChangedDelegate NodeSelectionChanged;

        private static void NodeSelectionListCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Flowchart flowchart = null;
            foreach (var pair in SelectedNodes)
            {
                if (pair.Value == sender)
                {
                    flowchart = FindFlowchart(pair.Key);
                }
            }

            NodeSelectionChanged?.Invoke(flowchart, sender as ObservableCollection<Guid>, args);
        }

        #endregion
    }
}
