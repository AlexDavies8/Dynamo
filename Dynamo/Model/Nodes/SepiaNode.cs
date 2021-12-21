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

    [Node("Filter/Sepia")]
    public class SepiaNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Input = null;

        [Port("Strength", true, typeof(string), true)]
        public string Strength = "1.0";

        [Port("Result", false, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Result = null;

        public SepiaNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.Sepia(float.Parse(Strength));
            });
        }
    }
}
