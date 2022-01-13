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
    [Node("Colour/Opacity", 504)]
    public class OpacityNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null, false)]
        public Image<Rgba32> Input = null;

        [Port("Opacity", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Opacity = 1.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public OpacityNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.Opacity(Opacity);
            });
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Opacity", Opacity.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Opacity = float.Parse(reader.GetAttribute("Opacity"));

            base.ReadXml(reader);
        }
    }
}
