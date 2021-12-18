using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using NodeGraph.Model;
using SixLabors.ImageSharp.PixelFormats;

namespace Dynamo.Model
{
    public class ViewportPanel : ModelBase
    {
        public Node DisplayedNode { get; private set; }
        public Port DisplayedPort { get; private set; }

        public ViewportPanel() : base()
        {
            GlobalState.ActiveNodeChanged += (node) =>
            {
                DisplayedNode = node;
                foreach (var port in DisplayedNode.OutputPorts)
                {
                    if (port.ValueType == typeof(SixLabors.ImageSharp.Image<Rgba32>)) // TODO: Better casting
                    {
                        DisplayedPort = port;
                    }
                }
            };
        }
    }
}
