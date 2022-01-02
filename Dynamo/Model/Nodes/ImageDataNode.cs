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
    [Node("Misc/Image Data")]
    public class ImageDataNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Input = null;

        [Port("Width", false, typeof(float), null)]
        public float Width = 2;

        [Port("Height", false, typeof(float), null)]
        public float Height = 2;

        public ImageDataNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Width = Input.Width;
            Height = Input.Height;
        }
    }
}
