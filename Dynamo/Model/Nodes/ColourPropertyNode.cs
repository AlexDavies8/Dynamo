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
    [Node("Input/Colour", 103)]
    public class ColourPropertyNode : ExecutableNode
    {
        [Port("Colour", true, typeof(System.Windows.Media.Color), typeof(ColourPropertyEditor), false)]
        public Color Colour = Color.White;

        [Port("Value", false, typeof(System.Windows.Media.Color), null)]
        public Color Value;

        public ColourPropertyNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            Value = Colour;
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Colour", Colour.ToHex());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Colour = Color.ParseHex(reader.GetAttribute("Colour"));

            base.ReadXml(reader);
        }
    }
}
