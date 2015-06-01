namespace Xoxo
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "schemaRef", Namespace = "http://www.xbrl.org/2003/linkbase")]
    public class SchemaReference : IEquatable<SchemaReference>
    {
        [XmlAttribute("type", Namespace = "http://www.w3.org/1999/xlink")]
        public string Type { get; set; }

        [XmlAttribute("href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Value { get; set; }

        public SchemaReference()
        {
        }

        public SchemaReference(string type, string value)
            : this()
        {
            this.Type = type;
            this.Value = value;
        }

        #region IEquatable implementation

        public bool Equals(SchemaReference other)
        {
            return this.Type.Equals(other.Type)
            && this.Value.Equals(other.Value);
        }

        #endregion
    }
}