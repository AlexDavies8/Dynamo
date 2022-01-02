using System;
using System.Collections.Generic;
using System.Text;
using NodeGraph;
using NodeGraph.Model;
using Dynamo.Controls.PropertyEditors;

namespace Dynamo.Model
{
    public class ExecutableNode : Node
    {
        public ExecutableNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
            OnPortChanged += (port) =>
            {
                if (port.IsInput || port.HasEditor)
                {
                    ExecutionManager.MarkDirty(this);
                }
            };
        }

        public override void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);

            ExecutionManager.MarkDirty(this);
        }

        public virtual void Execute() { }
    }
}
