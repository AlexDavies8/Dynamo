using NodeGraph.Model;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.PixelFormats;
using Dynamo.Controls.PropertyEditors;
using System.Xml;

namespace Dynamo.Model
{
    [Node("Output/Image", 200)]
    public class SaveImageNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null, false)]
        public Image<Rgba32> Input = null;

        [Port("Path", true, typeof(string), typeof(StringPropertyEditor))]
        public string Path = "";

        public SaveImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;
            if (!Directory.Exists(System.IO.Path.GetDirectoryName(Path))) return;
            
            Input.Save(Path);
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Path", Path);

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Path = reader.GetAttribute("Path");

            base.ReadXml(reader);
        }
    }
}
