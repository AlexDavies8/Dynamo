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
        public Type EditorType;

        public PortAttribute(string name, bool isInput, Type valueType, Type editorType)
        {
            Name = name;
            IsInput = isInput;
            ValueType = valueType;
            EditorType = editorType;
        }
    }
}
