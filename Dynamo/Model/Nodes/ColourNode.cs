using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Dynamo.Controls.PropertyEditors;
using System.Xml;

namespace Dynamo.Model
{
    [Node("Generator/Colour", 900)]
    public class ColourNode : ExecutableNode
    {
        [Port("Colour", true, typeof(Color), typeof(ColourPropertyEditor))]
        public Color Colour = Color.White;

        [Port("Width", true, typeof(int), typeof(IntPropertyEditor))]
        public int Width = 128;

        [Port("Height", true, typeof(int), typeof(IntPropertyEditor))]
        public int Height = 128;

        [Port("Image", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public ColourNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Width <= 0 || Height <= 0)
                return;

            var colour = Colour.ToPixel<Rgba32>();
            Result = new Image<Rgba32>(Width, Height);
            for (int y = 0; y < Result.Height; y++)
            {
                Span<Rgba32> pixelRowSpan = Result.GetPixelRowSpan(y);
                for (int x = 0; x < Result.Width; x++)
                {
                    pixelRowSpan[x] = colour;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Colour", Colour.ToHex());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Colour = Color.ParseHex(reader.GetAttribute("Colour"));
            Width = int.Parse(reader.GetAttribute("Width"));
            Height = int.Parse(reader.GetAttribute("Height"));

            base.ReadXml(reader);
        }
    }
}
