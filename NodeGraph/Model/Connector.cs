using System;
using System.Collections.Generic;
using System.Text;

namespace NodeGraph.Model
{
    class Connector : ModelBase
    {
        #region Fields

        public readonly Flowchart Owner;

        #endregion

        #region Properties

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
    }
}
