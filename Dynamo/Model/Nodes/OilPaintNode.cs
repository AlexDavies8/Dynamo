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

    [Node("Filter/Oil Paint")]
    public class OilPaintNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Input = null;

        [Port("Levels", true, typeof(int), typeof(IntPropertyEditor))]
        public int Levels = 10;

        [Port("Brush Size", true, typeof(int), typeof(IntPropertyEditor))]
        public int BrushSize = 15;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public OilPaintNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.OilPaint(Levels, BrushSize);
            });
        }
    }
}
