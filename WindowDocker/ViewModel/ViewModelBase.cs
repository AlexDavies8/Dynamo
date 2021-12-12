using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WindowDocker.Model;

namespace WindowDocker.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {

        #region Constructor

        public ViewModelBase(ModelBase model)
        {
            model.PropertyChanged += ModelPropertyChanged;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region ModelPropertyChanged

        protected virtual void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        #endregion

    }
}
