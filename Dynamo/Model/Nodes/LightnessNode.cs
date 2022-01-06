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
    [Node("Colour/Lightness")]
    public class LightnessNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Input = null;

        [Port("Lightness", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Lightness = 1.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public LightnessNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.Lightness(Lightness);
            });
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Lightness", Lightness.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Lightness = float.Parse(reader.GetAttribute("Lightness"));

            base.ReadXml(reader);
        }
    }
}
