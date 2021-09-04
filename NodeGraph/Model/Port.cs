using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NodeGraph.Model
{
    public class Port : ModelBase
    {
        #region Fields

        public readonly Node Owner;
        public readonly bool IsInput;

        #endregion

        #region Properties

        private PortViewModel _viewModel;
        public PortViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    RaisePropertyChanged("ViewModel");
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private ObservableCollection<Connector> _connectors = new ObservableCollection<Connector>();
        public ObservableCollection<Connector> Connectors
        {
            get => _connectors;
            set
            {
                if (_connectors != value)
                {
                    _connectors = value;
                    RaisePropertyChanged("Connectors");
                }
            }
        }

        public Type ValueType { get; private set; }

        #endregion

        #region Constructors

        public Port(Guid guid, Node owner, bool isInput, Type valueType) : base(guid)
        {
            Owner = owner;
            IsInput = isInput;
            ValueType = valueType;
        }

        #endregion
    }
}
