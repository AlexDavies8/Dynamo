using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Dynamo.Controls.PropertyEditors;
using System.Xml;

namespace Dynamo.Model
{
    [Node("Input/Image", 100)]
    public class ImageNode : ExecutableNode
    {
        [Port("Path", true, typeof(string), typeof(OpenPathPropertyEditor))]
        public string Path = "";

        [Port("Image", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public ImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Path == null) return;
            if (!File.Exists(Path)) return;

            Result = Image.Load<Rgba32>(Path);
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
