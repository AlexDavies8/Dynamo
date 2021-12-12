using NodeGraph.Model;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.Model
{
    public class SaveImageNode : ExecutableNode
    {
        [Port("", true, typeof(Image), null, false)]
        public Image Input;

        [Port("Path", true, typeof(string), "", true)]
        public string Path;

        public SaveImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input != null)
                Input.Save(Path);
        }
    }
}
