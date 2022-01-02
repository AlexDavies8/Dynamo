using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Dynamo.Converters;
using System.Diagnostics;

namespace Dynamo.Controls.PropertyEditors
{
    public class IntPropertyEditor : PropertyEditor
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(string),
                typeof(IntPropertyEditor),
                new PropertyMetadata(
                    "",
                    null,
                    CoerceValue),
                new ValidateValueCallback(IsValid));

        public IntPropertyEditor()
        {
            DataContext = this;
        }

        public static bool IsValid(object value)
        {
            string stringValue = value as string;
            if (stringValue == null || stringValue.Length == 0)
                return true;
            if (float.TryParse(stringValue, out float floatValue))
                return true;
            return false;
        }

        public static object CoerceValue(DependencyObject sender, object value)
        {
            if (sender is IntPropertyEditor)
            {
                string stringValue = value as string;
                if (float.TryParse(stringValue, out float floatValue))
                {
                    return ((int)floatValue).ToString();
                }
            }
            return null;
        }

        public override void SetValueBinding(Port port)
        {
            SetBinding(ValueProperty, GetBinding(port, new IntToStringConverter()));
        }
    }
}
