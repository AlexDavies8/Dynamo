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
        public bool Exposable;

        public PortAttribute(string name, bool isInput, Type valueType, Type editorType, bool exposable = true)
        {
            Name = name;
            IsInput = isInput;
            ValueType = valueType;
            EditorType = editorType;
            Exposable = exposable;
        }
    }
}
