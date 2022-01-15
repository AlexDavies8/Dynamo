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
    [Node("Input/Text", 102)]
    public class StringPropertyNode : ExecutableNode
    {
        [Port("Text", true, typeof(string), typeof(StringPropertyEditor), false)]
        public string Text = "";

        [Port("Value", false, typeof(string), typeof(StringPropertyEditor))]
        public string Value = "";

        public StringPropertyNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            Value = Text;
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Text", Text.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Text = reader.GetAttribute("Text");

            base.ReadXml(reader);
        }
    }
}
