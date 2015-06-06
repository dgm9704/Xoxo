namespace Diwen.Xbrl
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Xml;
	using System.Xml.Schema;
	using System.Xml.Serialization;

	public class TypedMember : IXmlSerializable, IEquatable<TypedMember>, IComparable<TypedMember>
	{
		[XmlIgnore]
		public XmlQualifiedName Dimension { get; set; }

		[XmlIgnore]
		public XmlQualifiedName Domain { get; set; }

		[XmlIgnore]
		public string Value { get; set; }

		public TypedMember()
		{

		}

		public TypedMember(XmlQualifiedName dimension, XmlQualifiedName domain, string value)
			: this()
		{
			this.Dimension = dimension;
			this.Domain = domain;
			this.Value = value;
		}

		#region IXmlSerializable implementation

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			if(reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			reader.MoveToContent();
			var dim = reader.GetAttribute("dimension");
			var idx = dim.IndexOf(':');
			var prefix = dim.Substring(0, idx);
			var dimNs = reader.LookupNamespace(prefix);
			this.Dimension = new XmlQualifiedName(dim, dimNs);
			reader.ReadStartElement();
			this.Domain = new XmlQualifiedName(reader.Name, reader.NamespaceURI);
			this.Value = reader.ReadElementString();
			reader.ReadEndElement();

		}

		public void WriteXml(XmlWriter writer)
		{
			if(writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteAttributeString("dimension", this.Dimension.Name);
			writer.WriteElementString(this.Domain.Prefix(), this.Domain.LocalName(), this.Domain.Namespace, this.Value);
		}

		public override int GetHashCode()
		{
			return this.Dimension.GetHashCode()
			^ this.Domain.GetHashCode()
			^ this.Value.GetHashCode();
		}

		#endregion

		#region IEquatable implementation

		public bool Equals(TypedMember other)
		{
			return other != null
			&& this.Dimension == other.Dimension
			&& this.Domain == other.Domain
			&& this.Value == other.Value;
		}

		#endregion

		#region IComparable implementation

		public int CompareTo(TypedMember other)
		{
			int result = 0;
			if(other == null)
			{
				result = -1;
			}
			else
			{
				result = string.Compare(this.Dimension.Name, other.Dimension.Name, StringComparison.OrdinalIgnoreCase);
				if(result == 0)
				{
					result = string.Compare(this.Domain.Name, other.Domain.Name, StringComparison.OrdinalIgnoreCase);
				}
				if(result == 0)
				{
					result = string.Compare(this.Value, other.Value, StringComparison.OrdinalIgnoreCase);
				}
			}
			return result;
		}

		#endregion

		public static bool operator ==(TypedMember left, TypedMember right)
		{
			// If both are null, or both are same instance, return true.
			if(object.ReferenceEquals(left, right))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if(((object)left == null) || ((object)right == null))
			{
				return false;
			}

			// Return true if the fields match:
			return left.Equals(right);
		}

		public static bool operator !=(TypedMember left, TypedMember right)
		{
			// If one is null, but not both, return true.
			if(((object)left == null) || ((object)right == null))
			{
				return true;
			}
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as TypedMember);
		}

		public static bool operator >(TypedMember left, TypedMember right)
		{
			// If both are null, or both are same instance, return false.
			if(object.ReferenceEquals(left, right))
			{
				return false;
			}
			return left != null && left.CompareTo(right) > 0;
		}

		public static bool operator <(TypedMember left, TypedMember right)
		{
			return right > left;
		}

		public int Compare(TypedMember other)
		{
			return this.CompareTo(other);
		}
	}
}