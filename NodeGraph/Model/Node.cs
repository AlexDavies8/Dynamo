using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace NodeGraph.Model
{
    class Node : ModelBase
    {

        #region Fields

        public readonly Flowchart Owner;
        protected FieldInfo _fieldInfo;
        protected PropertyInfo _propertyInfo;

        #endregion

        #region Properties

        protected string _header;
        public string Header
        {
            get => _header;
            set
            {
                if (_header != value)
                {
                    _header = value;
                    RaisePropertyChanged("Header");
                }
            }
        }

        protected bool _allowEditHeader;
        public bool AllowEditHeader
        {
            get => _allowEditHeader;
            set
            {
                if (_allowEditHeader != value)
                {
                    _allowEditHeader = value;
                    RaisePropertyChanged("AllowEditHeader");
                }
            }
        }

        protected double _x;
        public double X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    RaisePropertyChanged("X");
                }
            }
        }

        protected double _y;
        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    RaisePropertyChanged("Y");
                }
            }
        }

        protected ObservableCollection<Port> _inputPorts = new ObservableCollection<Port>();
        public ObservableCollection<Port> InputPorts
        {
            get => _inputPorts;
            set
            {
                if (_inputPorts != value)
                {
                    _inputPorts = value;
                    RaisePropertyChanged("InputPorts");
                }
            }
        }

        protected ObservableCollection<Port> _outputPorts = new ObservableCollection<Port>();
        public ObservableCollection<Port> OutputPorts
        {
            get => _outputPorts;
            set
            {
                if (_outputPorts != value)
                {
                    _outputPorts = value;
                    RaisePropertyChanged("OutputPorts");
                }
            }
        }

        #endregion

        #region Constructors

        public Node(Guid guid, Flowchart owner) : base(guid)
        {
            Owner = owner;
        }

        #endregion
    }
}
