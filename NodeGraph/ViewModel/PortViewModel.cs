using NodeGraph.Model;
using NodeGraph.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace NodeGraph.ViewModel
{
    public class PortViewModel : ViewModelBase
    {
        #region Fields

        public PortView View;

        #endregion

        #region Properties

        private Port _model;
        public Port Model
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

        public PortViewModel(Port port) : base(port)
        {
            Model = port;
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
