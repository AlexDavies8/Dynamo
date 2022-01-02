using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace NodeGraph.Model
{
    public class PropertyEditor : ContentControl
    {
        public virtual void SetValueBinding(Port port)
        {
            SetBinding(null, GetBinding(port));
        }

        protected virtual Binding GetBinding(Port port, IValueConverter converter = null, string propertyName = "Value")
        {
            return new Binding(propertyName)
            {
                Source = port,
                Mode = BindingMode.TwoWay,
                Converter = converter,
                UpdateSourceTrigger = UpdateSourceTrigger.Default,
                ValidatesOnDataErrors = true,
                ValidatesOnExceptions = true
            };
        }
    }
}
