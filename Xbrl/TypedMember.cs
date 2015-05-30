namespace Xoxo
{
	using System.Xml.Schema;
	using System.Xml;
	using System;
	using System.Xml.Serialization;
	using System.Collections.ObjectModel;

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

		public TypedMember(XmlQualifiedName dimension, XmlQualifiedName domain, string value) : this()
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
			writer.WriteAttributeString("dimension", this.Dimension.Name);
			writer.WriteElementString(this.Domain.Name, this.Value);
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
			return this.Dimension == other.Dimension
			&& this.Domain == other.Domain
			&& this.Value == other.Value;
		}

		#endregion

		#region IComparable implementation

		public int CompareTo(TypedMember other)
		{
			int result = this.Dimension.Name.CompareTo(other.Dimension.Name);
			if(result == 0)
			{
				result = this.Domain.Name.CompareTo(other.Domain.Name);
			}
			if(result == 0)
			{
				result = this.Value.CompareTo(other.Value);
			}
			return result;
		}

		#endregion
	}
}