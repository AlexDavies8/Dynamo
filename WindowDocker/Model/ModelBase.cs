﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WindowDocker.Model
{
    public class ModelBase : INotifyPropertyChanged
    {
        #region Constructor

        public ModelBase()
        {

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
