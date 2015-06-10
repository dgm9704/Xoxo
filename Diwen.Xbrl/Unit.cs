namespace Diwen.Xbrl
{
	using System;
	using System.Xml.Serialization;

	[Serializable]
	[XmlRoot(ElementName = "unit", Namespace = "http://www.xbrl.org/2003/instance")]
	public class Unit : IEquatable<Unit>
	{
		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlElement("measure")]
		public string Measure { get; set; }

		public Unit()
		{

		}

		public Unit(string id, string measure)
			: this()
		{
			this.Id = id;
			this.Measure = measure;
		}

		public override bool Equals(object obj)
		{
			var other = obj as Unit;
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

		public bool Equals(Unit other)
		{
			return other != null
			&& this.Id.Equals(other.Id, StringComparison.Ordinal)
			&& this.Measure.Equals(other.Measure, StringComparison.Ordinal);
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode() ^ this.Measure.GetHashCode();
		}

		#endregion
	}
}