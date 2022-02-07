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
    [Node("Generator/Noise/Simplex Noise", 911)]
    public class SimplexNoiseNode : ExecutableNode
    {
        [Port("Seed", true, typeof(int), typeof(IntPropertyEditor))]
        public int Seed = 0;

        [Port("Width", true, typeof(int), typeof(IntPropertyEditor))]
        public int Width = 128;

        [Port("Height", true, typeof(int), typeof(IntPropertyEditor))]
        public int Height = 128;

        [Port("Scale", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Scale = 0.1f;

        [Port("Octaves", true, typeof(int), typeof(IntPropertyEditor))]
        public int Octaves = 1;

        [Port("Persistance", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Persistance = 0.5f;

        [Port("Lacunarity", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Lacunarity = 2f;

        [Port("Image", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public SimplexNoiseNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Width <= 0 || Height <= 0)
                return;

            OpenSimplexNoise simplex = new OpenSimplexNoise(Seed);

            float[,] values = new float[Width, Height];
            float highest = 0f;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float octScale = 1f;
                    float octMul = 1f;
                    for (int i = 0; i < Octaves; i++)
                    {
                        values[x, y] += (float)(simplex.Evaluate(x * Scale * 0.01f * octScale, y * Scale * 0.01f * octScale) + 1f) * 0.5f * octMul;
                        octScale *= Lacunarity;
                        octMul *= Persistance;
                    }
                    if (values[x, y] > highest)
                        highest = values[x, y];
                }
            }
            Result = new Image<Rgba32>(Width, Height);
            for (int y = 0; y < Result.Height; y++)
            {
                Span<Rgba32> pixelRowSpan = Result.GetPixelRowSpan(y);
                for (int x = 0; x < Result.Width; x++)
                {
                    float v = values[x, y] / highest;
                    pixelRowSpan[x] = new Rgba32(v, v, v, 1f);
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Seed", Seed.ToString());
            writer.WriteAttributeString("Width", Width.ToString());
            writer.WriteAttributeString("Height", Height.ToString());
            writer.WriteAttributeString("Scale", Scale.ToString());
            writer.WriteAttributeString("Octaves", Octaves.ToString());
            writer.WriteAttributeString("Persistance", Persistance.ToString());
            writer.WriteAttributeString("Lacunarity", Lacunarity.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            Seed = int.Parse(reader.GetAttribute("Seed"));
            Width = int.Parse(reader.GetAttribute("Width"));
            Height = int.Parse(reader.GetAttribute("Height"));
            Scale = float.Parse(reader.GetAttribute("Scale"));
            Octaves = int.Parse(reader.GetAttribute("Octaves"));
            Persistance = float.Parse(reader.GetAttribute("Persistance"));
            Lacunarity = float.Parse(reader.GetAttribute("Lacunarity"));

            base.ReadXml(reader);
        }
    }
}
