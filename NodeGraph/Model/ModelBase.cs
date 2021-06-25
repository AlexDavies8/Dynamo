using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NodeGraph.Model
{
    class ModelBase : INotifyPropertyChanged
    {
        #region Properties

        public Guid Guid { get; private set; }

        #endregion

        #region Constructor

        public ModelBase()
        {

        }

        public ModelBase(Guid guid)
        {
            Guid = guid;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
