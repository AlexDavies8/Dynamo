﻿using NodeGraph.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using Dynamo.Controls.PropertyEditors;

namespace Dynamo.Model.Nodes
{
    [Node("Transform/Scale")]
    public class ResizeImageNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Input;

        [Port("Position", true, typeof(PositionType), typeof(EnumPropertyEditor))]
        public PositionType PositionType = PositionType.Fractional;

        [Port("X Scale", true, typeof(float), typeof(FloatPropertyEditor))]
        public float XScale = 1.0f;

        [Port("Y Scale", true, typeof(float), typeof(FloatPropertyEditor))]
        public float YScale = 1.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Output;

        public ResizeImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            int x = PositionType.GetPixelPosition(XScale, Input.Width);
            int y = PositionType.GetPixelPosition(YScale, Input.Height);

            Output = Input.Clone(image => image.Resize(x, y));
        }
    }
}
