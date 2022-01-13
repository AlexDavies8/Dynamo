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
    [Node("Colour/Contrast", 501)]
    public class ContrastNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null, false)]
        public Image<Rgba32> Input = null;

        [Port("Contrast", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Contrast = 1.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public ContrastNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.Contrast(Contrast);
            });
        }



        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Contrast", Contrast.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Contrast = float.Parse(reader.GetAttribute("Contrast"));

            base.ReadXml(reader);
        }
    }
}
