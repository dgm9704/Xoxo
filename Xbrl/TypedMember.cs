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
		public string Dimension { get; set; }

		[XmlIgnore]
		public string Domain { get; set; }

		[XmlIgnore]
		public string Value { get; set; }

		public TypedMember()
		{
	
		}

		public TypedMember(string dimension, string domain, string value) : this()
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
			this.Dimension = reader.GetAttribute("dimension");
			reader.ReadStartElement();
			this.Domain = reader.Name;
			this.Value = reader.ReadElementString();
			reader.ReadEndElement();

		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("dimension", this.Dimension);
			writer.WriteElementString(this.Domain, this.Value);
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
			var result = this.Dimension.CompareTo(other.Dimension);
			if(result == 0)
			{
				result = this.Domain.CompareTo(other.Domain);
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