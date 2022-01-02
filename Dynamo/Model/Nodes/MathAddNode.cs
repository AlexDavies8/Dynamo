using System;
using System.Collections.Generic;
using System.Text;
using NodeGraph.Model;
using NodeGraph;
using Dynamo.Controls.PropertyEditors;

namespace Dynamo.Model
{
    [Node("Math/Add")]
    class MathAddNode : ExecutableNode
    {
        [Port("A", true, typeof(float), typeof(FloatPropertyEditor))]
        public float A = 0;

        [Port("B", true, typeof(float), typeof(FloatPropertyEditor))]
        public float B = 0;

        [Port("Result", false, typeof(float), null)]
        public float Result = 0;

        public MathAddNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            Result = A + B;
        }
    }
}
