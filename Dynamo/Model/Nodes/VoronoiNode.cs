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
    [Node("Generator/Noise/Voronoi", 911)]
    public class VoronoiNode : ExecutableNode
    {
        [Port("Seed", true, typeof(int), typeof(IntPropertyEditor))]
        public int Seed = 0;

        [Port("Width", true, typeof(int), typeof(IntPropertyEditor))]
        public int Width = 128;

        [Port("Height", true, typeof(int), typeof(IntPropertyEditor))]
        public int Height = 128;

        [Port("X Density", true, typeof(float), typeof(FloatPropertyEditor))]
        public float DensityX = 4.0f;

        [Port("Y Density", true, typeof(float), typeof(FloatPropertyEditor))]
        public float DensityY = 4.0f;

        [Port("Randomness", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Randomness = 0.5f;

        [Port("Distance", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> DistanceImage = null;

        [Port("Cells", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> CellsImage = null;

        public VoronoiNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Width <= 0 || Height <= 0)
                return;

            Random random = new Random(Seed);

            DistanceImage = new Image<Rgba32>(Width, Height);
            CellsImage = new Image<Rgba32>(Width, Height);

            int cellsX = (int)Math.Ceiling(DensityX);
            int cellsY = (int)Math.Ceiling(DensityY);
            int cellWidth = Width / cellsX;
            int cellHeight = Height / cellsY;

            var points = new (float x, float y, Color colour)[cellsX + 2, cellsY + 2];
            for (int y = -1; y < cellsY + 1; y++)
            {
                for (int x = -1; x < cellsX + 1; x++)
                {
                    float rx = ((float)random.NextDouble() - 0.5f) * Randomness; // -0.5 to 0.5
                    float ry = ((float)random.NextDouble() - 0.5f) * Randomness; // -0.5 to 0.5
                    float px = cellWidth * x + cellWidth * (rx + 0.5f); // +0.5f to centre point in cell
                    float py = cellHeight * y + cellHeight * (ry + 0.5f); // +0.5f to centre point in cell
                    points[x + 1, y + 1] = (px, py, new Color(new Rgba32((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1f)));
                }
            }
            
            for (int y = 0; y < Height; y++)
            {
                var distanceRow = DistanceImage.GetPixelRowSpan(y);
                var cellsRow = CellsImage.GetPixelRowSpan(y);
                for (int x = 0; x < Width; x++)
                {
                    int cx = x / cellWidth;
                    int cy = y / cellHeight;
                    (float x, float y, Color colour) closest = points[cx + 1, cy + 1];
                    float minSqrDistance = float.MaxValue;
                    for (int py = Math.Max(cy - 1, -1); py <= Math.Min(cy + 1, cellsY); py++)
                    {
                        for (int px = Math.Max(cx - 1, -1); px <= Math.Min(cx + 1, cellsX); px++)
                        {
                            float dx = points[px + 1, py + 1].x - x;
                            float dy = points[px + 1, py + 1].y - y;
                            float sqrDistance = dx * dx + dy * dy;
                            if (sqrDistance < minSqrDistance)
                            {
                                closest = points[px + 1, py + 1];
                                minSqrDistance = sqrDistance;
                            }
                        }
                    }
                    float minDistance = (float)Math.Sqrt(minSqrDistance);
                    float v = minDistance / (float)Math.Sqrt(cellWidth * cellHeight);
                    distanceRow[x] = new Rgba32(v, v, v, 1f);
                    cellsRow[x] = closest.colour;
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
