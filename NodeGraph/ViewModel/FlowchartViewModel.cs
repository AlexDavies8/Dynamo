using NodeGraph.Model;
using NodeGraph.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace NodeGraph.ViewModel
{
    public class FlowchartViewModel : ViewModelBase
    {
        #region Fields

        public FlowchartView View;

        #endregion

        #region Properties

        private Flowchart _model;
        public Flowchart Model
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

        private ObservableCollection<NodeViewModel> _nodeViewModels = new ObservableCollection<NodeViewModel>();
        public ObservableCollection<NodeViewModel> NodeViewModels
        {
            get => _nodeViewModels;
            set
            {
                if (_nodeViewModels != value)
                {
                    _nodeViewModels = value;
                    RaisePropertyChanged("NodeViewModels");
                }
            }
        }

        private ObservableCollection<ConnectorViewModel> _connectorViewModels = new ObservableCollection<ConnectorViewModel>();
        public ObservableCollection<ConnectorViewModel> ConnectorViewModels
        {
            get => _connectorViewModels;
            set
            {
                if (_connectorViewModels != value)
                {
                    _connectorViewModels = value;
                    RaisePropertyChanged("ConnectorViewModels");
                }
            }
        }

        private double _selectionStartX;
        public double SelectionStartX
        {
            get => _selectionStartX;
            set
            {
                if (_selectionStartX != value)
                {
                    _selectionStartX = value;
                    RaisePropertyChanged("SelectionStartX");
                }
            }
        }

        private double _selectionStartY;
        public double SelectionStartY
        {
            get => _selectionStartY;
            set
            {
                if (_selectionStartY != value)
                {
                    _selectionStartY = value;
                    RaisePropertyChanged("SelectionStartY");
                }
            }
        }

        private double _selectionWidth;
        public double SelectionWidth
        {
            get => _selectionWidth;
            set
            {
                if (_selectionWidth != value)
                {
                    _selectionWidth = value;
                    RaisePropertyChanged("SelectionWidth");
                }
            }
        }

        private double _selectionHeight;
        public double SelectionHeight
        {
            get => _selectionHeight;
            set
            {
                if (_selectionHeight != value)
                {
                    _selectionHeight = value;
                    RaisePropertyChanged("SelectionHeight");
                }
            }
        }

        #endregion

        #region Constructor

        public FlowchartViewModel(Flowchart flowchart) : base(flowchart)
        {
            Model = flowchart;
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
