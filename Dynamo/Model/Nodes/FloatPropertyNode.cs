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
    [Node("Input/Number", 101)]
    public class FloatPropertyNode : ExecutableNode
    {
        [Port("Number", true, typeof(float), typeof(FloatPropertyEditor), false)]
        public float Number = 0.0f;

        [Port("Value", false, typeof(float), null)]
        public float Value = 0.0f;

        public FloatPropertyNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            Value = Number;
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Number", Number.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Number = float.Parse(reader.GetAttribute("Number"));

            base.ReadXml(reader);
        }
    }
}
