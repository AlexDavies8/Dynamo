using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using Microsoft.Win32;

namespace Dynamo.Controls.PropertyEditors
{
    public class OpenPathPropertyEditor : PropertyEditor
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
                typeof(OpenPathPropertyEditor),
                new PropertyMetadata(
                    ""),
                new ValidateValueCallback(IsValid));

        public OpenPathPropertyEditor()
        {
            DataContext = this;
        }

        public void BrowseForFile(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open File";
            if (openFileDialog.ShowDialog() == true)
                Value = openFileDialog.FileName;
        }

        public static bool IsValid(object value)
        {
            string stringValue = value as string;
            if (stringValue == null || stringValue.Length == 0)
                return true;

            if (!Directory.Exists(Path.GetDirectoryName(stringValue))) return false;

            return true;
        }

        public override void SetValueBinding(Port port)
        {
            SetBinding(ValueProperty, GetBinding(port));
        }
    }
}
