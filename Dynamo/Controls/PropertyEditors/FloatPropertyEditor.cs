using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Dynamo.Converters;

namespace Dynamo.Controls.PropertyEditors
{
    public class FloatPropertyEditor : PropertyEditor
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
                typeof(FloatPropertyEditor),
                new PropertyMetadata(
                    ""),
                new ValidateValueCallback(IsValid));

        public FloatPropertyEditor()
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

        public override void SetValueBinding(Port port)
        {
            SetBinding(ValueProperty, GetBinding(port, new FloatToStringConverter()));
        }
    }
}
