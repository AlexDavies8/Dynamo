using Dynamo.Model;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using SixLabors.ImageSharp;
using System.Windows.Controls;
using System.Windows;

namespace Dynamo.View
{
    public class GradientView : ContentControl
    {
        
        public Image<Rgba32> GradientImage
        {
            get => (Image<Rgba32>)GetValue(GradientImageProperty);
            set => SetValue(GradientImageProperty, value);
        }
        public static readonly DependencyProperty GradientImageProperty = DependencyProperty.Register("GradientImage", typeof(Image<Rgba32>), typeof(GradientView), new PropertyMetadata(null));
        
        public Gradient Model;

        public GradientView() : base()
        {
            DataContextChanged += GradientViewDataContextChanged;
            Loaded += GradientViewLoaded;
        }

        private void GradientViewLoaded(object sender, RoutedEventArgs e)
        {
            SynchroniseProperties();
        }

        private void GradientViewDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Model = DataContext as Gradient;

            if (Model == null) return;

            Model.PropertyChanged += (sender, e) => SynchroniseProperties();
            SynchroniseProperties();
        }

        void SynchroniseProperties()
        {
            if (Model == null) return;

            Image<Rgba32> newImage = new Image<Rgba32>(128, 20);
            for (int y = 0; y < newImage.Height; y++)
            {
                Span<Rgba32> pixels = newImage.GetPixelRowSpan(y);
                for (int x = 0; x < newImage.Width; x++)
                {
                    Rgba32 pixel = Model.Evaluate((float)x / pixels.Length).ToPixel<Rgba32>();
                    pixels[x] = pixel;
                }
            }
            GradientImage = newImage;
        }
    }
}
