using System;
using System.Collections.Generic;
using System.Text;

namespace NodeGraph.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OverrideStyleAttribute : Attribute
    {
        public string StyleName;

        public OverrideStyleAttribute(string styleName)
        {
            StyleName = styleName;
        }
    }
}
