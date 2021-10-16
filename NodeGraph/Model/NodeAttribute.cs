using System;
using System.Collections.Generic;
using System.Text;

namespace NodeGraph.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeAttribute : Attribute
    {
        public string Path;

        public NodeAttribute(string path)
        {
            Path = path;
        }
    }
}
