using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Dynamo.Model;
using System.ComponentModel;
using Dynamo.View;
using System.Diagnostics;

namespace Dynamo.ViewModel
{
    public class ViewportPanelViewModel : ViewModelBase
    {
        #region Fields

        public ViewportPanelView View;

        #endregion

        #region Properties

        private ViewportPanel _model;
        public ViewportPanel Model
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

        public ViewportPanelViewModel(ViewportPanel model) : base(model)
        {
            Model = model;
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
