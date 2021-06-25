using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WindowDocker.Model;

namespace WindowDocker.ViewModel
{
    public class SpanViewModel : ViewModelBase
    {
        #region Properties

        private Span _model;
        public Span Model
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

        #region Events

        protected override void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.ModelPropertyChanged(sender, e);

            RaisePropertyChanged(e.PropertyName);
        }

        #endregion
    }
}
