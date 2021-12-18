using NodeGraph.Model;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.Processing;

namespace Dynamo.Model
{
    public class HueShiftNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image), false)]
        public Image Input = null;

        [Port("Hue Shift", true, typeof(string), true)]
        public string Shift = "0.0";

        [Port("Result", false, typeof(Image), false)]
        public Image Result = null;

        public HueShiftNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.Hue(float.Parse(Shift));
            });
        }
    }
}
