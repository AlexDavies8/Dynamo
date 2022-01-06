using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Dynamo.Controls
{
    public class SharpImage : System.Windows.Controls.Image
    {
        public Image<Rgba32> RawImage
        {
            get => (Image<Rgba32>)GetValue(RawImageProperty);
            set => SetValue(RawImageProperty, value);
        }
        public static readonly DependencyProperty RawImageProperty = DependencyProperty.Register("RawImage", typeof(Image<Rgba32>), typeof(SharpImage), new PropertyMetadata(null));

        static SharpImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SharpImage), new FrameworkPropertyMetadata(typeof(SharpImage)));
        }
    }
}
