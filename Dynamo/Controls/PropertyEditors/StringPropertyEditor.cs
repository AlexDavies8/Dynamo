using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Dynamo.Controls.PropertyEditors
{
    public class StringPropertyEditor : PropertyEditor
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
                typeof(StringPropertyEditor),
                new PropertyMetadata(
                    ""),
                new ValidateValueCallback(IsValid));

        public StringPropertyEditor()
        {
            DataContext = this;
        }

        public static bool IsValid(object value)
        {
            return true;
        }

        public override void SetValueBinding(Port port)
        {
            SetBinding(ValueProperty, GetBinding(port));
        }
    }
}
