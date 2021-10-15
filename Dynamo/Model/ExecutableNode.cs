using System;
using System.Collections.Generic;
using System.Text;
using NodeGraph;
using NodeGraph.Model;

namespace Dynamo.Model
{
    public class ExecutableNode : Node
    {
        public ExecutableNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
            OnPortChanged += (port) => ExecutionManager.MarkDirty(this);
        }

        public override void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);

            ExecutionManager.MarkDirty(this);
        }

        public virtual void Execute() { }
    }
}
