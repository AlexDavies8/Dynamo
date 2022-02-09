using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using Microsoft.Win32;

namespace Dynamo.Controls.PropertyEditors
{
    public class SavePathPropertyEditor : PropertyEditor
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
                typeof(SavePathPropertyEditor),
                new PropertyMetadata(
                    ""),
                new ValidateValueCallback(IsValid));

        public SavePathPropertyEditor()
        {
            DataContext = this;
        }

        public void BrowseForFile(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save File";
            if (saveFileDialog.ShowDialog() == true)
                Value = saveFileDialog.FileName;
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
