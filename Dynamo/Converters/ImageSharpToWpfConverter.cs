using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace Dynamo.Converters
{
    [ValueConversion(typeof(Image<Rgba32>), typeof(WriteableBitmap))]
    public class ImageSharpToWpfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var image = value as Image<Rgba32>;

            if (image != null)
            {
                var bmp = new WriteableBitmap(image.Width, image.Height, image.Metadata.HorizontalResolution, image.Metadata.VerticalResolution, PixelFormats.Bgra32, null);

                bmp.Lock();
                try
                {
                    var backBuffer = bmp.BackBuffer;

                    for (var y = 0; y < image.Height; y++)
                    {
                        Span<Rgba32> buffer = image.GetPixelRowSpan(y);
                        for (var x = 0; x < image.Width; x++)
                        {
                            var backBufferPos = backBuffer + (y * image.Width + x) * 4;
                            Rgba32 rgba = buffer[x];
                            int color = rgba.A << 24 | rgba.R << 16 | rgba.G << 8 | rgba.B;
                            System.Runtime.InteropServices.Marshal.WriteInt32(backBufferPos, color);
                        }
                    }

                    bmp.AddDirtyRect(new Int32Rect(0, 0, image.Width, image.Height));
                }
                finally
                {
                    bmp.Unlock();
                }
                return bmp;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
