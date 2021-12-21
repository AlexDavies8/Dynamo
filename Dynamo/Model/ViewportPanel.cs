using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using NodeGraph.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Dynamo.Model
{
    public class ViewportPanel : ModelBase
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

        private Port _displayedPort;
        public Port DisplayedPort
        {
            get => _displayedPort;
            set
            {
                if (_displayedPort != value)
                {
                    _displayedPort = value;
                    RaisePropertyChanged("DisplayedPort");
                }
            }
        }

        public ViewportPanel() : base()
        {
            GlobalState.ActiveNodeChanged += (node) =>
            {
                DisplayedNode = node;
                foreach (var port in node.OutputPorts)
                {
                    if (port.ValueType == typeof(Image<Rgba32>)) // TODO: Better casting
                    {
                        DisplayedPort = port;
                    }
                }
            };
        }
    }
}
