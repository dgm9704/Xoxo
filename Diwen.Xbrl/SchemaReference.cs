namespace Diwen.Xbrl
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

		public override bool Equals(object obj)
		{
			var other = obj as SchemaReference;
			if(other != null)
			{
				return this.Equals(other);
			}
			else
			{
				return base.Equals(obj);
			}
		}

		#region IEquatable implementation

		public bool Equals(SchemaReference other)
		{
			return other != null
			&& this.Type.Equals(other.Type, StringComparison.Ordinal)
			&& this.Value.Equals(other.Value, StringComparison.Ordinal);
		}

		public override int GetHashCode()
		{
			return this.Type.GetHashCode() ^ this.Value.GetHashCode();
		}

		#endregion

	}
}