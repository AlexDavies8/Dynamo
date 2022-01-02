using System;
using System.Collections.Generic;
using System.Text;
using NodeGraph;
using NodeGraph.Model;
using Dynamo.Controls.PropertyEditors;

namespace Dynamo.Model
{
    public class ValueNode<T> : ExecutableNode
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

        public ValueNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Port port = NodeGraphManager.CreatePort("Value", Guid.NewGuid(), this, typeof(T), false, typeof(FloatPropertyEditor));
            port.PortValueChanged += PortValueChanged;
        }

        private void PortValueChanged(Port port, object prevValue, object newValue)
        {
            _value = (T)newValue;
        }

        public override void Execute()
        {
            // Nothing to do
        }
    }
}
