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
			return other != null
			&& this.Scheme.Equals(other.Scheme, StringComparison.Ordinal)
			&& this.Value.Equals(other.Value, StringComparison.Ordinal);
		}

		public override int GetHashCode()
		{
			return this.Scheme.GetHashCode() ^ this.Value.GetHashCode();
		}

		#endregion

		public override bool Equals(object obj)
		{
			var other = obj as Identifier;
			if(other != null)
			{
				return this.Equals(other);
			}
			else
			{
				return base.Equals(obj);
			}
		}
	}
}