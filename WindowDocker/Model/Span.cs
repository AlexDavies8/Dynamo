using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace WindowDocker.Model
{
    public class Span : ModelBase
    {
        #region Properties

        private Grid _childContainer;
        public Grid ChildContainer
        {
            get => _childContainer;
            set
            {
                if (_childContainer != value)
                {
                    _childContainer = value;
                    RaisePropertyChanged("ChildContainer");
                }
            }
        }

        private float _splitRatio = 1f;
        public float SplitRatio
        {
            get => _splitRatio;
            set
            {
                if (_splitRatio != value)
                {
                    _splitRatio = value;
                }
            }
        }

        #endregion

        #region Constructor

        public Span()
        {

        }

        #endregion
    }
}
