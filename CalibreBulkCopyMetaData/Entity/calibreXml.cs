using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace CalibreSetMetaData.Entity
{
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.idpf.org/2007/opf")]
    [XmlRoot(Namespace = "http://www.idpf.org/2007/opf", IsNullable = false)]
    public partial class calibreXml
    {

        /// <remarks/>
        public packageMetadata metadata { get; set; }

        /// <remarks/>
        public object manifest { get; set; }

        /// <remarks/>
        public object spine { get; set; }

        /// <remarks/>
        public object guide { get; set; }

        /// <remarks/>
        [XmlAttribute("unique-identifier")]
        public string uniqueidentifier { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public decimal version { get; set; }
    }

    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.idpf.org/2007/opf")]
    public partial class packageMetadata
    {

        /// <remarks/>
        [XmlElement("contributor", typeof(contributor), Namespace = "http://purl.org/dc/elements/1.1/")]
        [XmlElement("creator", typeof(creator), Namespace = "http://purl.org/dc/elements/1.1/")]
        [XmlElement("date", typeof(System.DateTime), Namespace = "http://purl.org/dc/elements/1.1/")]
        [XmlElement("identifier", typeof(identifier), Namespace = "http://purl.org/dc/elements/1.1/")]
        [XmlElement("language", typeof(string), Namespace = "http://purl.org/dc/elements/1.1/")]
        [XmlElement("publisher", typeof(string), Namespace = "http://purl.org/dc/elements/1.1/")]
        [XmlElement("subject", typeof(string), Namespace = "http://purl.org/dc/elements/1.1/")]
        [XmlElement("title", typeof(string), Namespace = "http://purl.org/dc/elements/1.1/")]
        [XmlElement("meta", typeof(packageMetadataMeta))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items { get; set; }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore()]
        public ItemsChoiceType[] ItemsElementName { get; set; }
    }

    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://purl.org/dc/elements/1.1/")]
    [XmlRoot(Namespace = "http://purl.org/dc/elements/1.1/", IsNullable = false)]
    public partial class contributor
    {

        /// <remarks/>
        [XmlAttribute("file-as", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.idpf.org/2007/opf")]
        public string fileas { get; set; }

        /// <remarks/>
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.idpf.org/2007/opf")]
        public string role { get; set; }

        /// <remarks/>
        [XmlText()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://purl.org/dc/elements/1.1/")]
    [XmlRoot(Namespace = "http://purl.org/dc/elements/1.1/", IsNullable = false)]
    public partial class creator
    {

        /// <remarks/>
        [XmlAttribute("file-as", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.idpf.org/2007/opf")]
        public string fileas { get; set; }

        /// <remarks/>
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.idpf.org/2007/opf")]
        public string role { get; set; }

        /// <remarks/>
        [XmlText()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://purl.org/dc/elements/1.1/")]
    [XmlRoot(Namespace = "http://purl.org/dc/elements/1.1/", IsNullable = false)]
    public partial class identifier
    {

        /// <remarks/>
        [XmlAttribute()]
        public string id { get; set; }

        /// <remarks/>
        [XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.idpf.org/2007/opf")]
        public string scheme { get; set; }

        /// <remarks/>
        [XmlText()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://www.idpf.org/2007/opf")]
    public partial class packageMetadataMeta
    {

        /// <remarks/>
        [XmlAttribute()]
        public string name { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        public string content { get; set; }
    }

    /// <remarks/>
    [System.Serializable()]
    [XmlType(Namespace = "http://www.idpf.org/2007/opf", IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        [XmlEnum("http://purl.org/dc/elements/1.1/:contributor")]
        contributor,

        /// <remarks/>
        [XmlEnum("http://purl.org/dc/elements/1.1/:creator")]
        creator,

        /// <remarks/>
        [XmlEnum("http://purl.org/dc/elements/1.1/:date")]
        date,

        /// <remarks/>
        [XmlEnum("http://purl.org/dc/elements/1.1/:identifier")]
        identifier,

        /// <remarks/>
        [XmlEnum("http://purl.org/dc/elements/1.1/:language")]
        language,

        /// <remarks/>
        [XmlEnum("http://purl.org/dc/elements/1.1/:publisher")]
        publisher,

        /// <remarks/>
        [XmlEnum("http://purl.org/dc/elements/1.1/:subject")]
        subject,

        /// <remarks/>
        [XmlEnum("http://purl.org/dc/elements/1.1/:title")]
        title,

        /// <remarks/>
        meta,
    }
}
