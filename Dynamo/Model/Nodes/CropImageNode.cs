using NodeGraph.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using Dynamo.Controls.PropertyEditors;

namespace Dynamo.Model
{
    [Node("Transform/Crop")]
    public class CropImageNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Input;

        [Port("X Scale", true, typeof(float), typeof(FloatPropertyEditor))]
        public float XScale = 1.0f;

        [Port("Y Scale", true, typeof(float), typeof(FloatPropertyEditor))]
        public float YScale = 1.0f;

        [Port("X Offset", true, typeof(float), typeof(FloatPropertyEditor))]
        public float XOffset = 0.0f;

        [Port("Y Offset", true, typeof(float), typeof(FloatPropertyEditor))]
        public float YOffset = 0.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Output;

        public CropImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            int x = (int)(XOffset * Input.Width);
            int y = (int)(YOffset * Input.Height);
            int width = (int)(XScale * Input.Width);
            int height = (int)(YScale * Input.Height);

            Output = Input.Clone(image => image.Crop(new Rectangle(x, y, width, height)));
        }
    }
}