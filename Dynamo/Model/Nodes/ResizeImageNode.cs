using NodeGraph.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.Model
{
    public class ResizeImageNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Input;

        [Port("Scale", true, typeof(string), true)]
        public string Scale = "1.0";

        [Port("Result", false, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Output;

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
