using NodeGraph.Model;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Dynamo.Controls.PropertyEditors;
using System.Xml;

namespace Dynamo.Model
{
    [Node("Colour/Gradient Map", 505)]
    public class GradientMapNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null, false)]
        public Image<Rgba32> Input = null;

        [Port("Gradient", true, typeof(Gradient), typeof(GradientPropertyEditor))]
        public Gradient Gradient = new Gradient();

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public GradientMapNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            Result = Input.Clone(x =>
            {
                x.ProcessPixelRowsAsVector4(row =>
                {
                    for (int x = 0; x < row.Length; x++)
                    {
                        float lightness = row[x].X * 0.2126f + row[x].Y * 0.7152f + row[x].Z * 0.0722f;
                        Rgba32 c = Gradient.Evaluate(lightness).ToPixel<Rgba32>();
                        row[x] = new Vector4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
                    }
                });
            });
        }

        public override void WriteXml(XmlWriter writer)
        {
            //writer.WriteAttributeString("Saturation", Saturation.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            //Saturation = float.Parse(reader.GetAttribute("Saturation"));

            base.ReadXml(reader);
        }
    }
}
