using NodeGraph;
using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.Model
{
    [Node("Debug/Log")]
    public class DebugValueNode<T> : ExecutableNode
    {
        private object _value;
        public object Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }

        public DebugValueNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Port port = NodeGraphManager.CreatePort(typeof(T).Name, Guid.NewGuid(), this, typeof(T), true, false);
            port.PortValueChanged += PortValueChanged;
        }

        private void PortValueChanged(Port port, object prevValue, object newValue)
        {
            _value = (T)newValue;
        }

        public override void Execute()
        {
            System.Diagnostics.Debug.WriteLine(Value);
        }
    }
}
