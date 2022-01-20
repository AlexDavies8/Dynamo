using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Dynamo.Converters;
using System.Diagnostics;
using Dynamo.Model;

namespace Dynamo.Controls.PropertyEditors
{
    public class GradientPropertyEditor : PropertyEditor
    {
        public Gradient Value
        {
            get { return (Gradient)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(Gradient),
                typeof(GradientPropertyEditor),
                new PropertyMetadata(
                    new Gradient(),
                    null,
                    CoerceValue),
                new ValidateValueCallback(IsValid));

        public GradientPropertyEditor()
        {
            DataContext = this;
        }

        public static bool IsValid(object value)
        {
            return true;
            /*
            string stringValue = value as string;
            if (stringValue == null || stringValue.Length == 0)
                return true;
            if (float.TryParse(stringValue, out float floatValue))
                return true;
            return false;
            */
        }

        public static object CoerceValue(DependencyObject sender, object value)
        {
            return value;
            /*
            if (sender is IntPropertyEditor)
            {
                string stringValue = value as string;
                if (float.TryParse(stringValue, out float floatValue))
                {
                    return ((int)floatValue).ToString();
                }
            }
            return null;
            */
        }

        public override void SetValueBinding(Port port)
        {
            SetBinding(ValueProperty, GetBinding(port));
        }
    }
}
