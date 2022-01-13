using System;
using System.Collections.Generic;
using System.Text;

namespace NodeGraph.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeAttribute : Attribute
    {
        public string Path;
        public int Order;

        public NodeAttribute(string path, int order = int.MaxValue)
        {
            Path = path;
            Order = order;
        }
    }
}
