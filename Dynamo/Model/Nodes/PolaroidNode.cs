using NodeGraph.Model;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Dynamo.Controls.PropertyEditors;

namespace Dynamo.Model
{

    [Node("Filter/Colour/Polaroid", 701)]
    public class PolaroidNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null, false)]
        public Image<Rgba32> Input = null;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public PolaroidNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.Polaroid();
            });
        }
    }
}
