using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace NodeGraph.Model
{
    public class ModelBase : INotifyPropertyChanged, IXmlSerializable
    {
        #region Properties

        public Guid Guid { get; private set; }

        #endregion

        #region Constructor

        public ModelBase()
        {

        }

        public ModelBase(Guid guid)
        {
            Guid = guid;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IXmlSerializable

        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            // Type and GUID will always be read before instance is created, so no need to implement readers
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Guid", Guid.ToString());
            writer.WriteAttributeString("Type", GetType().AssemblyQualifiedName);
        }

        #endregion
    }
}
