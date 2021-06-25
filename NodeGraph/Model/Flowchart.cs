using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NodeGraph.Model
{
    class Flowchart : ModelBase
    {
        #region Properties

        protected ObservableCollection<Node> _nodes = new ObservableCollection<Node>();
        public ObservableCollection<Node> Nodes
        {
            get => _nodes;
            set
            {
                if (_nodes != value)
                {
                    _nodes = value;
                    RaisePropertyChanged("Nodes");
                }
            }
        }

        protected ObservableCollection<Connector> _connectors = new ObservableCollection<Connector>();
        public ObservableCollection<Connector> Connectors
        {
            get => _connectors;
            set
            {
                if (_connectors != value)
                {
                    _connectors = value;
                    RaisePropertyChanged("Connectors");
                }
            }
        }

        #endregion

        #region Constructors

        #endregion
    }
}
