using NodeGraph.Model;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace Dynamo.Model
{
    [Node("Colour/Opacity")]
    public class OpacityNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Input = null;

        [Port("Opacity", true, typeof(string), true)]
        public string Opacity = "1.0";

        [Port("Result", false, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Result = null;

        public OpacityNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.Opacity(float.Parse(Opacity));
            });
        }
    }
}
