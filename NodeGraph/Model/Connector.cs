using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace NodeGraph.Model
{
    public class Connector : ModelBase
    {
        #region Fields

        public readonly Flowchart Owner;

        #endregion

        #region Properties

        private ConnectorViewModel _viewModel;
        public ConnectorViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    RaisePropertyChanged("ViewModel");
                }
            }
        }

        private Port _startPort;
        public Port StartPort
        {
            get => _startPort;
            set
            {
                if (_startPort != value)
                {
                    _startPort = value;
                    RaisePropertyChanged("StartPort");
                }
            }
        }

        private Port _endPort;
        public Port EndPort
        {
            get => _endPort;
            set
            {
                if (_endPort != value)
                {
                    _endPort = value;
                    RaisePropertyChanged("EndPort");
                }
            }
        }

        #endregion

        #region Constructors

        public Connector(Guid guid, Flowchart owner) : base(guid)
        {
            Owner = owner;
        }

        #endregion

        #region Methods

        public bool IsConnectedTo(Port port)
        {
            return (port == StartPort) || (port == EndPort);
        }

        #endregion

        #region Serialization

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            StartPort = NodeGraphManager.FindPort(Guid.Parse(reader.GetAttribute("StartPort")));
            EndPort = NodeGraphManager.FindPort(Guid.Parse(reader.GetAttribute("EndPort")));
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            // Readonly data read before instance is created
            writer.WriteAttributeString("Owner", Owner.Guid.ToString());
            // End of readonly data

            if (StartPort != null)
                writer.WriteAttributeString("StartPort", StartPort.Guid.ToString());
            if (EndPort != null)
                writer.WriteAttributeString("EndPort", EndPort.Guid.ToString());
        }

        #endregion
    }
}
