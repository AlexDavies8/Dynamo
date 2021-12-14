using System;
using System.Collections.Generic;
using System.Text;
using NodeGraph.Model;
using NodeGraph;

namespace Dynamo.Model
{
    [Node("Math/Add")]
    class MathAddNode : ExecutableNode
    {
        [Port("A", true, typeof(int), false)]
        public int A = 0;

        [Port("B", true, typeof(int), false)]
        public int B = 0;

        [Port("Result", false, typeof(int), false)]
        public int Result = 0;

        public MathAddNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            Result = A + B;
        }
    }
}
