using NodeGraph.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace NodeGraph.Model
{
    public class Flowchart : ModelBase
    {
        #region Properties

        protected FlowchartViewModel _viewModel;
        public FlowchartViewModel ViewModel
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

        protected ObservableCollection<Node> _nodes = new ObservableCollection<Node>();
        public ObservableCollection<Node> Nodes
        {
            get => _nodes;
            set
            {
                if (_nodes != value)
                {
                    _nodes = value;
                    RaisePropertyChanged("Nodes");
                }
            }
        }

        protected ObservableCollection<Connector> _connectors = new ObservableCollection<Connector>();
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

        #endregion

        #region Constructors

        #endregion

        #region Serialization

        public override void ReadXml(XmlReader xmlReader)
        {
            base.ReadXml(xmlReader);

            xmlReader.ReadMultiple(new string[] { "Nodes", "Connectors" },
                ("Node",
                reader =>
                {
                    Guid guid = Guid.Parse(reader.GetAttribute("Guid"));
                    Type type = Type.GetType(reader.GetAttribute("Type"));
                    Guid owner = Guid.Parse(reader.GetAttribute("Owner"));

                    Flowchart flowchart = NodeGraphManager.FindFlowchart(owner);
                    Node node = NodeGraphManager.CreateNode("", guid, flowchart, type, 0, 0, true);
                    node.ReadXml(reader);
                    node.OnCreate();
                }),
                ("Connector",
                reader =>
                {
                    Guid guid = Guid.Parse(reader.GetAttribute("Guid"));
                    Guid owner = Guid.Parse(reader.GetAttribute("Owner"));

                    Flowchart flowchart = NodeGraphManager.FindFlowchart(owner);
                    Connector connector = NodeGraphManager.CreateConnector(guid, flowchart);
                    connector.ReadXml(reader);
                })
            );
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("Nodes");
            foreach (var node in Nodes)
            {
                writer.WriteStartElement("Node");
                node.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Connectors");
            foreach (var connector in Connectors)
            {
                writer.WriteStartElement("Connector");
                connector.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
