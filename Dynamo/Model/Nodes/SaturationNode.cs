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
    [Node("Colour/Saturation")]
    public class SaturationNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Input = null;

        [Port("Saturation", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Saturation = 1.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public SaturationNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.Saturate(Saturation);
            });
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Saturation", Saturation.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Saturation = float.Parse(reader.GetAttribute("Saturation"));

            base.ReadXml(reader);
        }
    }
}
