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

    [Node("Blur/Box")]
    public class BoxBlurNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Input = null;

        [Port("Radius", true, typeof(string), true)]
        public string Radius = "10";

        [Port("Result", false, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Result = null;

        public BoxBlurNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.BoxBlur(int.Parse(Radius));
            });
        }
    }
}
