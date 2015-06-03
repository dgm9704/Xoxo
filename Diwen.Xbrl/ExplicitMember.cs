namespace Diwen.Xbrl
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

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
            var result = false;
            if (other != null)
            {
                if (this.Dimension == other.Dimension)
                {
                    if (this.Value == other.Value)
                    {
                        result = true;
                    }
                }

            }

            return result;
        }

        #endregion

        #region IComparable implementation

        public int CompareTo(ExplicitMember other)
        {
            int result = 0;
            if (other == null)
            {
                result = 1;
            }
            else
            {
                result = string.Compare(this.Dimension.Name, other.Dimension.Name, StringComparison.OrdinalIgnoreCase);
                if (result == 0)
                {
                    result = string.Compare(this.Value.Name, other.Value.Name, StringComparison.OrdinalIgnoreCase);
                }
            }

            return result;
        }

        #endregion

        public static bool operator ==(ExplicitMember left, ExplicitMember right)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            // Return true if the fields match:
            return left.Equals(right);
        }

        public static bool operator !=(ExplicitMember left, ExplicitMember right)
        {
            // If one is null, but not both, return true.
            if (((object)left == null) || ((object)right == null))
            {
                return true;
            }
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ExplicitMember);
        }

        public static bool operator >(ExplicitMember left, ExplicitMember right)
        {
            // If both are null, or both are same instance, return false.
            if (object.ReferenceEquals(left, right))
            {
                return false;
            }

            return left != null && left.CompareTo(right) > 0;
        }

        public static bool operator <(ExplicitMember left, ExplicitMember right)
        {
            // If both are null, or both are same instance, return false.
            if (object.ReferenceEquals(left, right))
            {
                return false;
            }

            return left != null && left.CompareTo(right) < 0;
        }
    }
}