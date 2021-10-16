using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;

namespace Dynamo.Model
{
    class ImageNode : ExecutableNode
    {
        [Port("Path", true, typeof(string), "", true)]
        public string Path;

        [Port("", false, typeof(Image), null, false)]
        public Image Image;

        public ImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            Image = Image.Load(Path);
        }
    }
}
