using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace NodeGraph.Model
{
    public class Node : ModelBase
    {

        #region Fields

        public readonly Flowchart Owner;
        protected FieldInfo _fieldInfo;
        protected PropertyInfo _propertyInfo;

        #endregion

        #region Properties

        protected NodeViewModel _viewModel;
        public NodeViewModel ViewModel
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

        protected int _zIndex = 1;
        public int ZIndex
        {
            get => _zIndex;
            set
            {
                if (_zIndex != value)
                {
                    _zIndex = value;
                    RaisePropertyChanged("ZIndex");
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

        #region Events

        public Action<Port> OnPortChanged { get; set; }

        public virtual void OnCreate() 
        {
            InputPorts.CollectionChanged += PortCollectionChanged;
            OutputPorts.CollectionChanged += PortCollectionChanged;
        }

        private void PortCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var item in e.NewItems)
                    ((Port)item).PropertyChanged += PortChanged;

            if (e.OldItems != null)
                foreach (var item in e.OldItems)
                    ((Port)item).PropertyChanged -= PortChanged;
        }

        private void PortChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPortChanged((Port)sender);
        }

        #endregion

        #region Serialization

        public override void ReadXml(XmlReader xmlReader)
        {
            base.ReadXml(xmlReader);

            Header = xmlReader.GetAttribute("Header");
            AllowEditHeader = bool.Parse(xmlReader.GetAttribute("AllowEditHeader"));

            X = double.Parse(xmlReader.GetAttribute("X"));
            Y = double.Parse(xmlReader.GetAttribute("Y"));
            ZIndex = int.Parse(xmlReader.GetAttribute("ZIndex"));

            xmlReader.ReadMultiple(new string[] { "InputPorts", "OutputPorts" },
                ("Port",
                reader =>
                {
                    string name = reader.GetAttribute("Name");
                    Guid guid = Guid.Parse(reader.GetAttribute("Guid"));
                    Type type = Type.GetType(reader.GetAttribute("Type"));
                    bool isInput = bool.Parse(reader.GetAttribute("IsInput"));
                    Type valueType = Type.GetType(reader.GetAttribute("ValueType"));
                    string editorTypeString = reader.GetAttribute("EditorType");
                    Type editorType = editorTypeString != null ? Type.GetType(editorTypeString) : null;

                    Guid ownerGuid = Guid.Parse(reader.GetAttribute("Owner"));
                    Node node = NodeGraphManager.FindNode(ownerGuid);

                    if (node != null)
                    {
                        PropertyInfo matchProperty = null;
                        FieldInfo matchField = null;
                        PortAttribute matchAttribute = null;

                        Type nodeType = node.GetType();

                        PropertyInfo[] propertyInfos = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (PropertyInfo propertyInfo in propertyInfos)
                        {
                            PortAttribute[] portAttributes = propertyInfo.GetCustomAttributes(typeof(PortAttribute), false) as PortAttribute[];
                            foreach (PortAttribute portAttribute in portAttributes)
                            {
                                if (portAttribute.Name == name)
                                {
                                    matchProperty = propertyInfo;
                                    matchAttribute = portAttribute;
                                }    
                            }
                        }

                        FieldInfo[] fieldInfos = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                        foreach (FieldInfo fieldInfo in fieldInfos)
                        {
                            PortAttribute[] portAttributes = fieldInfo.GetCustomAttributes(typeof(PortAttribute), false) as PortAttribute[];
                            foreach (PortAttribute portAttribute in portAttributes)
                            {
                                if (portAttribute.Name == name)
                                {
                                    matchField = fieldInfo;
                                    matchAttribute = portAttribute;
                                }
                            }
                        }

                        Port port = null;
                        if (matchField != null)
                        {
                            port = NodeGraphManager.CreatePort(name, guid, node, valueType, isInput, editorType, () => matchField.GetValue(node), matchAttribute.Exposable);
                            port.PortValueChanged += (Port port, object prevValue, object newValue) =>
                            {
                                matchField.SetValue(node, Convert.ChangeType(newValue, port.ValueType));
                                node.OnPortChanged?.Invoke(port);
                            };
                        }
                        else if (matchProperty != null)
                        {
                            port = NodeGraphManager.CreatePort(name, guid, node, valueType, isInput, editorType, () => matchProperty.GetValue(node), matchAttribute.Exposable);
                            port.PortValueChanged += (Port port, object prevValue, object newValue) =>
                            {
                                matchProperty.SetValue(node, Convert.ChangeType(newValue, port.ValueType));
                                node.OnPortChanged?.Invoke(port);
                            };
                        }
                        else
                        {
                            port = NodeGraphManager.CreatePort(name, guid, node, valueType, isInput, editorType, exposable: matchAttribute.Exposable);
                            Debug.WriteLine($"Could not find Port {name} on Type {nodeType.FullName}");
                        }
                        port.ReadXml(reader);
                    }
                })
            );
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            // Readonly Data required for creating instance (not read here)
            writer.WriteAttributeString("Owner", Owner.Guid.ToString());
            // End of Readonly Data

            writer.WriteAttributeString("Header", Header);
            writer.WriteAttributeString("AllowEditHeader", AllowEditHeader.ToString());

            writer.WriteAttributeString("X", X.ToString());
            writer.WriteAttributeString("Y", Y.ToString());
            writer.WriteAttributeString("ZIndex", ZIndex.ToString());

            writer.WriteStartElement("InputPorts");
            foreach (var port in InputPorts)
            {
                writer.WriteStartElement("Port");
                port.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("OutputPorts");
            foreach (var port in OutputPorts)
            {
                writer.WriteStartElement("Port");
                port.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
