using NodeGraph.Model;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Dynamo.Controls.PropertyEditors;
using System.Xml;

namespace Dynamo.Model
{

    [Node("Blur/Gaussian", 601)]
    public class GaussianBlurNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null, false)]
        public Image<Rgba32> Input = null;

        [Port("Radius", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Radius = 10.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public GaussianBlurNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.GaussianBlur(Radius);
            });
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Radius", Radius.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Radius = int.Parse(reader.GetAttribute("Radius"));

            base.ReadXml(reader);
        }
    }
}
