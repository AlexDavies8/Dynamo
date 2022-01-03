using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Data;

namespace Dynamo.Converters
{
    public class EnumToStringConverter : IValueConverter
	{
        private Type _targetType;

        public EnumToStringConverter(Type targetType)
        {
            _targetType = targetType;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            return myEnum.ToString();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse(_targetType, value as string);
        }
    }
}
