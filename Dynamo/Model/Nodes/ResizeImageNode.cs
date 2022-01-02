using NodeGraph.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using Dynamo.Controls.PropertyEditors;

namespace Dynamo.Model
{
    [Node("Transform/Scale")]
    public class ResizeImageNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Input;

        [Port("Scale", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Scale = 1.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Output;

        public ResizeImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Output = Input.Clone(x => x.Resize((int)(Input.Width * Scale), (int)(Input.Height * Scale)));
        }
    }
}
