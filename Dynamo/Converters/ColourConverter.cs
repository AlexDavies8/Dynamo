using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Dynamo.Converters
{
	[ValueConversion(typeof(System.Windows.Media.Color), typeof(Color))]
	public class ColourConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Color colour = (Color)value;
			Rgba32 pixel = colour.ToPixel<Rgba32>();
			return System.Windows.Media.Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var colour = (System.Windows.Media.Color)value;
			if (colour != null)
				return new Color(new Rgba32(colour.R, colour.G, colour.B, colour.A));
			return Color.White;
		}
	}
}