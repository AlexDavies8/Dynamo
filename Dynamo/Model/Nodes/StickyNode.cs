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
    [OverrideStyle("StickyNodeStyle")]
    [Node("Note", 0)]
    public class StickyNode : Node
    {
        private string _text = "";
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }

        public StickyNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Text", Text);

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            _text = reader.GetAttribute("Text");

            base.ReadXml(reader);
        }
    }
}
