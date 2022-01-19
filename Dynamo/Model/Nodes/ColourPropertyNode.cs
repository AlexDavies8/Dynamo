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
        [Port("Text", true, typeof(System.Windows.Media.Color), typeof(ColourPropertyEditor), false)]
        public System.Windows.Media.Color Colour = System.Windows.Media.Colors.White;

        [Port("Value", false, typeof(System.Windows.Media.Color), null)]
        public System.Windows.Media.Color Value;

        public ColourPropertyNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            Value = Colour;
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Colour", Colour.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            //Colour = System.Windows.Media.Color.Fr(reader.GetAttribute("Colour"));

            base.ReadXml(reader);
        }
    }
}
