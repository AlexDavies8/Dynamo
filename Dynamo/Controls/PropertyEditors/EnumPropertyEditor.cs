using Dynamo.Converters;
using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Dynamo.Controls.PropertyEditors
{
    public class EnumPropertyEditor : PropertyEditor
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
                typeof(EnumPropertyEditor),
                new PropertyMetadata(
                    ""),
                new ValidateValueCallback(IsValid));

        public string[] EnumNames { get; set; } = new string[1];

        public EnumPropertyEditor()
        {
            DataContext = this;
        }

        public static bool IsValid(object value)
        {
            return true;
        }

        public override void SetValueBinding(Port port)
        {
            EnumNames = Enum.GetNames(port.ValueType);
            SetBinding(ValueProperty, GetBinding(port, new EnumToStringConverter(port.ValueType)));
            UpdateLayout();
        }
    }
}
