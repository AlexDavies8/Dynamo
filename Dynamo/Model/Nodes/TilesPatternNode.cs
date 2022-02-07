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
    [Node("Generator/Pattern/Tiles", 920)]
    public class TilesPatternNode : ExecutableNode
    {
        [Port("Width", true, typeof(int), typeof(IntPropertyEditor))]
        public int Width = 128;

        [Port("Height", true, typeof(int), typeof(IntPropertyEditor))]
        public int Height = 128;

        [Port("X Count", true, typeof(int), typeof(IntPropertyEditor))]
        public int CountX = 4;

        [Port("Y Count", true, typeof(int), typeof(IntPropertyEditor))]
        public int CountY = 4;

        [Port("Spacing", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Spacing = 0.05f;

        [Port("Offset", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Offset = 0f;

        [Port("Coverage", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Coverage = 1f;

        [Port("Seed", true, typeof(int), typeof(IntPropertyEditor))]
        public int Seed = 0;

        [Port("Image", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public TilesPatternNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Width <= 0 || Height <= 0)
                return;

            OpenSimplexNoise noise = new OpenSimplexNoise(Seed);

            float tileWidth = Width / CountX;
            float tileHeight = Height / CountY;
            float spacing = tileWidth * Spacing;

            float coverage = Coverage - 0.5f;
            coverage *= coverage;
            coverage *= Math.Sign(Coverage - 0.5f) * 4f / 3f;
            coverage += 0.5f;
            Result = new Image<Rgba32>(Width, Height);
            for (int y = 0; y < Result.Height; y++)
            {
                Span<Rgba32> pixelRowSpan = Result.GetPixelRowSpan(y);
                for (int x = 0; x < Result.Width; x++)
                {
                    int tx = (int)((x + Offset * tileWidth * 0.5f * ((int)(y / tileHeight) % 2)) / tileWidth);
                    int ty = (int)(y / tileHeight);
                    bool outside = (noise.Evaluate(tx, ty) + 1f) * 0.5f > coverage || (x + Offset * tileWidth * 0.5f * ((int)(y / tileHeight) % 2)) % tileWidth <= spacing || y % tileHeight <= spacing;

                    pixelRowSpan[x] = outside ? Color.Black : Color.White;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Seed", Seed.ToString());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString());
            writer.WriteAttributeString("CountX", CountX.ToString());
            writer.WriteAttributeString("CountY", CountY.ToString());
            writer.WriteAttributeString("Spacing", Spacing.ToString());
            writer.WriteAttributeString("Offset", Offset.ToString());
            writer.WriteAttributeString("Coverage", Coverage.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Seed = int.Parse(reader.GetAttribute("Seed"));
            Width = int.Parse(reader.GetAttribute("Width"));
            Height = int.Parse(reader.GetAttribute("Height"));
            CountX = int.Parse(reader.GetAttribute("CountX"));
            CountY = int.Parse(reader.GetAttribute("CountY"));
            Spacing = float.Parse(reader.GetAttribute("Spacing"));
            Offset = float.Parse(reader.GetAttribute("Offset"));
            Coverage = float.Parse(reader.GetAttribute("Coverage"));

            base.ReadXml(reader);
        }
    }
}
