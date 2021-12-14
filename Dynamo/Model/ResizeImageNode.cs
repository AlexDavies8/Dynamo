using NodeGraph.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.Model
{
    public class ResizeImageNode : ExecutableNode
    {
        [Port("Original", true, typeof(Image), false)]
        public Image Input;

        [Port("Scale", true, typeof(string), true)]
        public string Scale = "1.0";

        [Port("Resized", false, typeof(Image), false)]
        public Image Output;

        public ResizeImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Output = Input.Clone(x => x.Resize((int)(Input.Width * double.Parse(Scale)), (int)(Input.Height * double.Parse(Scale))));
        }
    }
}
