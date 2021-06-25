using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using WindowDocker.ViewModel;

namespace WindowDocker.View
{
    public class SpanView : ContentControl
    {
        #region Properties

        public SpanViewModel ViewModel { get; private set; }

        #endregion

        #region Constructor

        public SpanView() : base()
        {
            DataContext = ViewModel;
        }

        #endregion
    }
}
