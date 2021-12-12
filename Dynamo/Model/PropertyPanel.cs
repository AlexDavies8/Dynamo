using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.Model
{
    public class PropertyPanel : ModelBase
    {
        private Node _displayedNode;
        public Node DisplayedNode
        {
            get => _displayedNode;
            set
            {
                if (_displayedNode != value)
                {
                    _displayedNode = value;
                    RaisePropertyChanged("DisplayedNode");
                }
            }
        }

        public PropertyPanel() : base()
        {
            GlobalState.ActiveNodeChanged += (node) =>
            {
                DisplayedNode = node;
            };
        }
    }
}
