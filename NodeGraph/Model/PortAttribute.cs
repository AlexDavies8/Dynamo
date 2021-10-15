using System;
using System.Collections.Generic;
using System.Text;

namespace NodeGraph.Model
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PortAttribute : Attribute
    {
        public string Name;
        public bool IsInput;
        public Type ValueType;
        public object DefaultValue;
        public bool HasEditor;

        public PortAttribute(string name, bool isInput, Type valueType, object defaultValue, bool hasEditor)
        {
            Name = name;
            IsInput = isInput;
            ValueType = valueType;
            DefaultValue = defaultValue;
            HasEditor = hasEditor;
        }
    }
}
