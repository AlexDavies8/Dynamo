using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NodeGraph.Model
{
    public class Port : ModelBase
    {
        #region Events

        public delegate void PortValueChangedDelegate(Port port, object prevValue, object newValue);
        public event PortValueChangedDelegate PortValueChanged;

        protected virtual void OnPortValueChanged(object prevValue, object newValue)
        {
            PortValueChanged?.Invoke(this, prevValue, newValue);
        }

        #endregion

        #region Fields

        public readonly Node Owner;
        public readonly bool IsInput;
        public readonly bool HasEditor;

        private readonly bool _fromAttribute;

        #endregion

        #region Properties

        private object _value;
        public object Value
        {
            get
            {
                if (_fromAttribute)
                {
                    object value = _getValue();
                    if (_value != value)
                    {
                        _value = value;
                        RaisePropertyChanged("Value");
                    }
                }
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    OnPortValueChanged(_value, value);
                    _value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }

        private Func<object> _getValue;

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

        private bool _exposed;
        public bool Exposed
        {
            get => _exposed;
            set
            {
                if (_exposed != value)
                {
                    _exposed = value;
                    RaisePropertyChanged("Exposed");
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

        public Port(Guid guid, Node owner, bool isInput, Type valueType, bool hasEditor, Func<object> getValue = null) : base(guid)
        {
            Owner = owner;
            IsInput = isInput;
            ValueType = valueType;
            HasEditor = hasEditor;

            _fromAttribute = getValue != null;
            _getValue = getValue;

            Connectors.CollectionChanged += ConnectorsCollectionChanged;
        }

        private void ConnectorsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("Connectors");
        }

        #endregion
    }
}
