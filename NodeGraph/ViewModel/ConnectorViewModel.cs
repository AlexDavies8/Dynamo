using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NodeGraph.ViewModel
{
    public class ConnectorViewModel : ViewModelBase
    {
        #region Properties

        private Connector _model;
        public Connector Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    RaisePropertyChanged("Model");
                }
            }
        }

        #endregion

        #region Constructor

        public ConnectorViewModel(Connector connector) : base(connector)
        {
            Model = connector;
        }

        #endregion
    }
}
