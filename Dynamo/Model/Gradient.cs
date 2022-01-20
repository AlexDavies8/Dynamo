using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Dynamo.Model
{
    public class Gradient : ModelBase
    {
        private ObservableCollection<GradientTag> _tags;
        public ObservableCollection<GradientTag> Tags
        {
            get => _tags;
            set
            {
                if (_tags != value)
                {
                    _tags = value;
                    RaisePropertyChanged("Tags");
                }
            }
        }

        public Gradient() : base()
        {
            Tags.CollectionChanged += (sender, e) => RaisePropertyChanged("Tags");

            _tags = new ObservableCollection<GradientTag>();
            CreateTag(Color.Black, 0f);
            CreateTag(Color.White, 1f);
        }

        public void CreateTag(Color colour, float time)
        {
            var tag = new GradientTag(colour, time);
            Tags.Add(tag);
        }

        public Color Evaluate(float time)
        {
            for (int i = 0; i < Tags.Count - 1; i++)
            {
                if (Tags[i + 1].Time > time)
                {
                    float max = Tags[i + 1].Time;
                    float min = Tags[i].Time;
                    float frac = (time - min) / (max - min);
                    return BlendColours(Tags[i].Colour, Tags[i + 1].Colour, frac);
                }
            }
            return Color.Black;
        }

        Color BlendColours(Color colourA, Color colourB, float frac)
        {
            var colA = colourA.ToPixel<Rgba32>();
            var colB = colourB.ToPixel<Rgba32>();
            float r = colA.R * frac + colB.R * (1f - frac);
            float g = colA.G * frac + colB.G * (1f - frac);
            float b = colA.B * frac + colB.B * (1f - frac);
            float a = colA.A * frac + colB.A * (1f - frac);
            return new Color(new Rgba32(r, g, b, a));
        }
    }

    public class GradientTag
    {
        public Color Colour { get; set; }
        public float Time { get; set; }

        public GradientTag(Color colour, float time)
        {
            Colour = colour;
            Time = time;
        }
    }
}
