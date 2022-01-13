using System;
using System.Collections.Generic;
using System.Text;
using NodeGraph;
using NodeGraph.Model;
using Dynamo.Controls.PropertyEditors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Windows;
using System.Xml;

namespace Dynamo.Model
{
    [OverrideStyle("PreviewNodeStyle")]
    public class ExecutableNode : Node
    {
        private bool _previewEnabled = true;
        public bool PreviewEnabled
        {
            get => _previewEnabled;
            set
            {
                _previewEnabled = value;
                RaisePropertyChanged("PreviewVisibility");
            }
        }

        public Visibility PreviewVisibility => PreviewEnabled && _previewPort != null ? Visibility.Visible : Visibility.Collapsed;

        private Image<Rgba32> _previewImage;
        public Image<Rgba32> PreviewImage
        { 
            get => _previewImage;
            set
            {
                if (_previewImage != value)
                {
                    _previewImage = value;
                    RaisePropertyChanged("PreviewImage");
                }
            }
        }

        private Port _previewPort = null;

        public ExecutableNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
            OnPortChanged += (port) =>
            {
                if (port.IsInput)
                {
                    ExecutionManager.MarkDirty(this);
                }
            };
        }

        public override void OnCreate()
        {
            base.OnCreate();

            foreach (var port in OutputPorts)
            {
                if (port.ValueType == typeof(Image<Rgba32>))
                {
                    _previewPort = port;
                    break;
                }
            }

            if (_previewPort == null)
            {
                foreach (var port in InputPorts)
                {
                    if (port.ValueType == typeof(Image<Rgba32>))
                    {
                        _previewPort = port;
                        break;
                    }
                }
            }

            ExecutionManager.MarkDirty(this);
        }

        public override void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);

            //Debug.WriteLine($"Marked {Header} Dirty from Property {propertyName}");
            //ExecutionManager.MarkDirty(this);
        }

        public virtual void Execute() { }

        public virtual void OnPostExecute()
        {
            if (_previewPort != null)
                PreviewImage = (Image<Rgba32>)_previewPort.Value;
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ShowPreview", PreviewEnabled.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            PreviewEnabled = bool.Parse(reader.GetAttribute("ShowPreview"));

            base.ReadXml(reader);
        }
    }
}
