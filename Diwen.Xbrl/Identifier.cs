namespace Diwen.Xbrl
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class Identifier : IEquatable<Identifier>
    {
        [XmlAttribute("scheme", Namespace = "http://www.xbrl.org/2003/instance")]
        public string Scheme { get; set; }

        [XmlText]
        public string Value { get; set; }

        public Identifier()
        {
        }

        public Identifier(string scheme, string value)
            : this()
        {
            this.Scheme = scheme;
            this.Value = value;
        }

        #region IEquatable implementation

        public bool Equals(Identifier other)
        {
            return this.Scheme.Equals(other.Scheme)
            && this.Value.Equals(other.Value);
        }

        #endregion
    }
}