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
    [Node("Input/Gradient", 104)]
    public class GradientPropertyNode : ExecutableNode
    {
        [Port("Gradient", true, typeof(Gradient), typeof(GradientPropertyEditor), false)]
        public Gradient Gradient = new Gradient();

        [Port("Value", false, typeof(Gradient), null)]
        public Gradient Value;

        public GradientPropertyNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            Value = Gradient;
        }

        public override void WriteXml(XmlWriter writer)
        {
            //writer.WriteAttributeString("Colour", Colour.ToHex());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            //Colour = Color.ParseHex(reader.GetAttribute("Colour"));

            base.ReadXml(reader);
        }
    }
}
