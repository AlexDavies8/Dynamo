﻿using NodeGraph.Model;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace Dynamo.Model
{

    [Node("Blur/Bokeh")]
    public class BokehBlurNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Input = null;

        [Port("Radius", true, typeof(string), true)]
        public string Radius = "10";

        [Port("Kernel Size", true, typeof(string), true)]
        public string KernelSize = "1";

        [Port("Gamma", true, typeof(string), true)]
        public string Gamma = "1.0";

        [Port("Result", false, typeof(Image<Rgba32>), false)]
        public Image<Rgba32> Result = null;

        public BokehBlurNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.BokehBlur(int.Parse(Radius), int.Parse(KernelSize), float.Parse(Gamma));
            });
        }
    }
}