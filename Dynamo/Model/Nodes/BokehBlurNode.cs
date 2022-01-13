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
    [Node("Blur/Bokeh", 602)]
    public class BokehBlurNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null, false)]
        public Image<Rgba32> Input = null;

        [Port("Radius", true, typeof(int), typeof(IntPropertyEditor))]
        public int Radius = 10;

        [Port("Kernel Size", true, typeof(int), typeof(IntPropertyEditor))]
        public int KernelSize = 1;

        [Port("Gamma", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Gamma = 1.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public BokehBlurNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.BokehBlur(Radius, KernelSize, Gamma);
            });
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Radius", Radius.ToString());
            writer.WriteAttributeString("KernelSize", KernelSize.ToString());
            writer.WriteAttributeString("Gamma", Gamma.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Radius = int.Parse(reader.GetAttribute("Radius"));
            KernelSize = int.Parse(reader.GetAttribute("KernalSize"));
            Gamma = float.Parse(reader.GetAttribute("Gamma"));

            base.ReadXml(reader);
        }
    }
}
