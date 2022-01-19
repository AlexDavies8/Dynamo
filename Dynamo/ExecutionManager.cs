using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Dynamo.Model;
using NodeGraph;
using NodeGraph.Model;

namespace Dynamo
{
    public static class ExecutionManager
    {
        public static Action OnPostExecute;
        private static List<ExecutableNode> _dirtyNodes = new List<ExecutableNode>();
        private static bool _executing;
        public static bool AutoExecute = true;

        public static void ResolveDirtyNodes()
        {
            _executing = true;
            List<ExecutableNode> cleanNodes = new List<ExecutableNode>();
            while (_dirtyNodes.Count > 0)
            {
                foreach(var node in _dirtyNodes)
                {
                    if (!HasDirtyDependencies(node))
                    {
                        SynchroniseConnectors(node);

                        node.Execute();

                        cleanNodes.Add(node);
                    }
                }
                foreach (var node in cleanNodes)
                {
                    _dirtyNodes.Remove(node);
                    node.OnPostExecute();
                }
            }
            OnPostExecute?.Invoke();
            _executing = false;
        }

        // Synchronise inputs with their connected outputs
        // TODO: Possibly change this to synchronise output ports to their connected inputs after
        // their parent node has finished executing so that node's connections are only synced after they
        // have finished executing - only dirty nodes resync properties
        private static bool SynchroniseConnectors(Node node)
        {
            foreach (var port in node.InputPorts)
            {
                foreach (var connector in port.Connectors)
                {
                    if (connector.EndPort != null && connector.StartPort != null) // Dont sync nodes currently being dragged
                        connector.EndPort.Value = connector.StartPort.Value;
                    else
                        return false;
                }
            }
            return true;
        }

        private static bool HasDirtyDependencies(ExecutableNode node)
        {
            foreach (var port in node.InputPorts)
            {
                foreach (var connector in port.Connectors)
                {
                    if (_dirtyNodes.Contains(connector.StartPort?.Owner as ExecutableNode))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void MarkDirty(ExecutableNode node)
        {
            if (_executing) return;

            MarkDirtyRecurse(node);

            if (AutoExecute)
                ResolveDirtyNodes();
        }

        private static void MarkDirtyRecurse(ExecutableNode node)
        {
            MarkDirtyInternal(node);
            foreach (var port in node.OutputPorts)
            {
                foreach (var connection in port.Connectors)
                {
                    if (connection.EndPort != null)
                    {
                        MarkDirtyRecurse(connection.EndPort.Owner as ExecutableNode);
                    }
                }
            }
        }

        private static void MarkDirtyInternal(ExecutableNode node)
        {
            if (!_dirtyNodes.Contains(node))
                _dirtyNodes.Add(node);
        }
    }
}
