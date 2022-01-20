using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            _tags = new ObservableCollection<GradientTag>();
            Tags.CollectionChanged += TagsCollectionChanged;

            CreateTag(Color.Black, 0f);
            CreateTag(Color.Red, 0.7f);
            CreateTag(Color.White, 1f);
        }

        private void TagsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("Tags");
            if (e.OldItems != null)
            {
                foreach (var obj in e.OldItems)
                {
                    var tag = obj as GradientTag;
                    if (tag != null)
                        tag.PropertyChanged -= TagPropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (var obj in e.NewItems)
                {
                    var tag = obj as GradientTag;
                    if (tag != null)
                        tag.PropertyChanged += TagPropertyChanged;
                }
            }
        }

        private void TagPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged("Tags");
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
                    return BlendColours(Tags[i + 1].Colour, Tags[i].Colour, frac);
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
            return new Color(new Rgba32((byte)r, (byte)g, (byte)b, (byte)a));
        }
    }

    public class GradientTag : ModelBase
    {
        private Color _colour;
        public Color Colour
        {
            get => _colour;
            set
            {
                if (_colour != value)
                {
                    _colour = value;
                    RaisePropertyChanged("Colour");
                }
            }
        }

        private float _time;
        public float Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    RaisePropertyChanged("Time");
                }
            }
        }

        public GradientTag(Color colour, float time) : base()
        {
            Colour = colour;
            Time = time;
        }
    }
}
