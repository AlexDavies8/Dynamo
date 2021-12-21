using NodeGraph.Model;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.PixelFormats;

namespace Dynamo.Model
{
    [Node("Output/Image")]
    public class SaveImageNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Input = null;

        [Port("Path", true, typeof(string), true)]
        public string Path = "";

        public SaveImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(Path))) return;
            
            Input.Save(Path);
        }
    }
}
