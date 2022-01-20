using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Xml;

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
        public readonly bool Exposable;
        public bool HasEditor => PropertyEditorType != null;

        public readonly Type PropertyEditorType;

        private readonly bool _fromAttribute;

        public bool IsExposable => Exposable;

        #endregion

        #region Properties

        private object _value;
        public object Value
        {
            get
            {
                if (_fromAttribute)
                {
                    return _getValue();
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

        public Port(Guid guid, Node owner, bool isInput, Type valueType, Type editorType, Func<object> getValue = null, bool exposable = true) : base(guid)
        {
            Owner = owner;
            IsInput = isInput;
            ValueType = valueType;
            PropertyEditorType = editorType;

            Exposed = false;

            _fromAttribute = getValue != null;
            _getValue = getValue;

            Connectors.CollectionChanged += ConnectorsCollectionChanged;
        }

        private void ConnectorsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("Connectors");
            OnPortValueChanged(Value, Value);
        }

        #endregion

        #region Serialization

        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            Name = reader.GetAttribute("Name");
            Exposed = bool.Parse(reader.GetAttribute("Exposed"));
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            // Readonly Data read before instance is created
            writer.WriteAttributeString("Owner", Owner.Guid.ToString());
            writer.WriteAttributeString("IsInput", IsInput.ToString());
            writer.WriteAttributeString("ValueType", ValueType.AssemblyQualifiedName);
            if (PropertyEditorType != null)
                writer.WriteAttributeString("EditorType", PropertyEditorType.AssemblyQualifiedName);
            // End of readonly data

            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Exposed", Exposed.ToString());
        }

        #endregion
    }
}
