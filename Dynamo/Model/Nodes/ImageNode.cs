using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Dynamo.Model
{
    [Node("Input/Image")]
    public class ImageNode : ExecutableNode
    {
        [Port("Path", true, typeof(string), true)]
        public string Path = "";

        [Port("Image", false, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Result = null;

        public ImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Path == null) return;
            if (!File.Exists(Path)) return;

            Result = Image.Load<Rgba32>(Path);
        }
    }
}
