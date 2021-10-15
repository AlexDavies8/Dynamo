using System;
using System.Collections.Generic;
using System.Text;
using NodeGraph.Model;
using NodeGraph;

namespace Dynamo.Model
{
    class MathAddNode : ExecutableNode
    {
        [Port("A", true, typeof(int), 0, false)]
        public int A;

        [Port("B", true, typeof(int), 0, false)]
        public int B;

        [Port("Result", false, typeof(int), 0, false)]
        public int Result;

        public MathAddNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            Result = A + B;
        }
    }
}
