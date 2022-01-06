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

        private Image<Rgba32> _displayedImage;
        public Image<Rgba32> DisplayedImage
        {
            get => _displayedImage;
            set
            {
                if (_displayedImage != value)
                {
                    _displayedImage = value;
                    RaisePropertyChanged("DisplayedImage");
                }
            }
        }

        private bool _locked;
        public bool Locked
        {
            get => _locked;
            set
            {
                if (_locked != value)
                {
                    _locked = value;
                    RaisePropertyChanged("Locked");
                }
            }
        }

        public ViewportPanel() : base()
        {
            GlobalState.ActiveNodeChanged += UpdateDisplayedPort;
        }

        private void UpdateDisplayedPort(Node node)
        {
            if (Locked) return;

            if (node != null)
                DisplayedNode = node;
        }

        public override void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);

            if (propertyName == "Locked")
            {
                UpdateDisplayedPort(GlobalState.ActiveNode);
            }
        }
    }
}
