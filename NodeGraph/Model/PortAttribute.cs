using System;
using System.Collections.Generic;
using System.Text;

namespace NodeGraph.Model
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    class PortAttribute : Attribute
    {
        #region Fields

        public string Name;
        public bool IsInput;

        #endregion
    }
}
