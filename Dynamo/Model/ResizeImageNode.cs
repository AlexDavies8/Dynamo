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
        [Port("Image", true, typeof(Image), null, false)]
        public Image Input;

        [Port("Scale", true, typeof(string), "1.0", true)]
        public string Scale;

        [Port("Image", false, typeof(Image), null, false)]
        public Image Output;

        public ResizeImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input != null)
                Output = Input.Clone(x => x.Resize((int)(Input.Width * double.Parse(Scale)), (int)(Input.Height * double.Parse(Scale))));
        }
    }
}
