using NodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Dynamo.Model;
using System.ComponentModel;
using Dynamo.View;
using System.Collections.ObjectModel;
using System.Diagnostics;
using NodeGraph.ViewModel;

namespace Dynamo.ViewModel
{
    public class PropertyPanelViewModel : ViewModelBase
    {
        #region Fields

        public PropertyPanelView View;

        #endregion

        #region Properties

        private PropertyPanel _model;
        public PropertyPanel Model
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

        private ObservableCollection<PortViewModel> _portViewModels = new ObservableCollection<PortViewModel>();
        public ObservableCollection<PortViewModel> PortViewModels
        {
            get => _portViewModels;
            set
            {
                if (_portViewModels != value)
                {
                    _portViewModels = value;
                    RaisePropertyChanged("PortViewModels");
                }
            }
        }

        #endregion

        #region Constructor

        public PropertyPanelViewModel(PropertyPanel model) : base(model)
        {
            Model = model;
            PortViewModels.CollectionChanged += (sender, e) => RaisePropertyChanged("Ports");
        }

        #endregion

        #region Events

        protected override void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.ModelPropertyChanged(sender, e);

            if (e.PropertyName == "DisplayedNode")
            {
                UpdatePropertyNames();
            }

            RaisePropertyChanged(e.PropertyName);
        }

        #endregion

        #region PropertyNames

        private void UpdatePropertyNames()
        {
            PortViewModels.Clear();

            var node = Model.DisplayedNode;
            if (node != null)
            {
                foreach (var port in node.InputPorts)
                {
                    PortViewModels.Add(port.ViewModel);
                }
                foreach (var port in node.OutputPorts)
                {
                    PortViewModels.Add(port.ViewModel);
                }
            }
        }

        #endregion
    }
}
