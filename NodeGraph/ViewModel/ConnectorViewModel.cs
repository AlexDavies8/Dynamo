using NodeGraph.Model;
using NodeGraph.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NodeGraph.ViewModel
{
    public class ConnectorViewModel : ViewModelBase
    {
        #region Fields

        public ConnectorView View;

        #endregion

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

        #region Events

        protected override void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.ModelPropertyChanged(sender, e);

            RaisePropertyChanged(e.PropertyName);
        }

        #endregion
    }
}
