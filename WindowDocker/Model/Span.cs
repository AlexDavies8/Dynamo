using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using WindowDocker.ViewModel;

namespace WindowDocker.Model
{
    public class Span : ModelBase
    {
        #region Properties

        private SpanViewModel _viewModel;
        public SpanViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel != value)
                {
                    RaisePropertyChanged("ViewModel");
                    _viewModel = value;
                }
            }
        }

        private ObservableCollection<Span> _children;
        public ObservableCollection<Span> Children
        {
            get => _children;
            set
            {
                if (_children != value)
                {
                    RaisePropertyChanged("Children");
                    _children = value;
                }
            }
        }

        private ObservableCollection<double> _ratios;
        public ObservableCollection<double> Ratios
        {
            get => _ratios;
            set
            {
                if (_ratios != value)
                {
                    RaisePropertyChanged("Ratios");
                    _ratios = value;
                }
            }
        }

        #endregion

        #region Constructor

        public Span() : base()
        {

        }

        #endregion
    }
}
