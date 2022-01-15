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
    [Node("Generator/Noise/Value Noise", 900)]
    public class ValueNoiseNode : ExecutableNode
    {
        [Port("Seed", true, typeof(int), typeof(IntPropertyEditor))]
        public int Seed = 0;

        [Port("Width", true, typeof(int), typeof(IntPropertyEditor))]
        public int Width = 128;

        [Port("Height", true, typeof(int), typeof(IntPropertyEditor))]
        public int Height = 128;

        [Port("Image", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public ValueNoiseNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Width <= 0 || Height <= 0)
                return;

            Random random = new Random(Seed);

            Result = new Image<Rgba32>(Width, Height);
            for (int y = 0; y < Result.Height; y++)
            {
                Span<Rgba32> pixelRowSpan = Result.GetPixelRowSpan(y);
                for (int x = 0; x < Result.Width; x++)
                {
                    float v = (float)random.NextDouble();
                    pixelRowSpan[x] = new Rgba32(v, v, v, 1f);
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Seed", Seed.ToString());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Seed = int.Parse(reader.GetAttribute("Seed"));
            Width = int.Parse(reader.GetAttribute("Width"));
            Height = int.Parse(reader.GetAttribute("Height"));

            base.ReadXml(reader);
        }
    }
}
