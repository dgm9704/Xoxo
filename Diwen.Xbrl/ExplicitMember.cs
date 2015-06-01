namespace Diwen.Xbrl
{
    using System;
    using System.Xml.Serialization;
    using System.Xml;

    [Serializable]
    [XmlRoot(ElementName = "explicitMember", Namespace = "http://xbrl.org/2006/xbrldi")]
    public class ExplicitMember : IEquatable<ExplicitMember>, IComparable<ExplicitMember>
    {
        [XmlAttribute("dimension", Namespace = "http://xbrl.org/2006/xbrldi")]
        public XmlQualifiedName Dimension { get; set; }

        [XmlText]
        public XmlQualifiedName Value { get; set; }

        public ExplicitMember()
        {
        }

        public ExplicitMember(XmlQualifiedName dimension, XmlQualifiedName value)
            : this()
        {
            this.Dimension = dimension;
            this.Value = value;
        }

        public override int GetHashCode()
        {
            return this.Dimension.GetHashCode()
            ^ this.Value.GetHashCode();
        }

        #region IEquatable implementation

        public bool Equals(ExplicitMember other)
        {
            return this.Dimension == other.Dimension
            && this.Value == other.Value;
        }

        #endregion

        #region IComparable implementation

        public int CompareTo(ExplicitMember other)
        {
            int result = this.Dimension.Name.CompareTo(other.Dimension.Name);
            if (result == 0)
            {
                result = this.Value.Name.CompareTo(other.Value.Name);
            }
            return result;
        }

        #endregion
    }
}