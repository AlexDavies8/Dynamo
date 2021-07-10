using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace NodeGraph.ViewModel
{
    public class NodeViewModel : ViewModelBase
    {
        #region Fields



        #endregion

        #region Properties

        private Node _model;
        public Node Model
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

        private ObservableCollection<PortViewModel> _inputPortViewModels = new ObservableCollection<PortViewModel>();
        public ObservableCollection<PortViewModel> InputPortViewModels
        {
            get => _inputPortViewModels;
            set
            {
                if (_inputPortViewModels != value)
                {
                    _inputPortViewModels = value;
                    RaisePropertyChanged("InputPortViewModels");
                }
            }
        }

        private ObservableCollection<PortViewModel> _outputPortViewModels = new ObservableCollection<PortViewModel>();
        public ObservableCollection<PortViewModel> OutputPortViewModels
        {
            get => _outputPortViewModels;
            set
            {
                if (_outputPortViewModels != value)
                {
                    _outputPortViewModels = value;
                    RaisePropertyChanged("OutputPortViewModels");
                }
            }
        }

        #endregion

        #region Constructors

        public NodeViewModel(Node node) : base(node)
        {
            Model = node;
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
