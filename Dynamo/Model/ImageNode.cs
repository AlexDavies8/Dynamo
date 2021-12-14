using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SixLabors.ImageSharp;

namespace Dynamo.Model
{
    public class ImageNode : ExecutableNode
    {
        [Port("Path", true, typeof(string), true)]
        public string Path = "";

        [Port("", false, typeof(Image), false)]
        public Image Image = null;

        public ImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Path == null) return;
            if (!File.Exists(Path)) return;

            Image = Image.Load(Path);
        }
    }
}
