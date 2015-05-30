namespace Xoxo
{
	using System;
	using System.Xml.Serialization;
	using System.Collections.ObjectModel;

	[Serializable]
	[XmlRoot(ElementName = "explicitMember", Namespace = "http://xbrl.org/2006/xbrldi")]
	public class ExplicitMember : IEquatable<ExplicitMember>, IComparable<ExplicitMember>
	{
		[XmlAttribute("dimension", Namespace = "http://xbrl.org/2006/xbrldi")]
		public string Dimension { get; set; }

		[XmlText]
		public string Value { get; set; }

		public ExplicitMember()
		{

		}

		public ExplicitMember(string dimension, string value) : this()
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
			int result = this.Dimension.CompareTo(other.Dimension);
			if(result == 0)
			{
				result = this.Value.CompareTo(other.Value);
			}
			return result;
		}

		#endregion
	}
}