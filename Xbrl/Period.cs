namespace Xoxo
{
	using System;
	using System.Xml.Serialization;
	using System.Collections.ObjectModel;

	[Serializable]
	[XmlRoot(ElementName = "period", Namespace = "http://www.xbrl.org/2003/instance")]
	public class Period : IEquatable<Period>
	{
		[XmlElement(ElementName = "instant", DataType = "date", Namespace = "http://www.xbrl.org/2003/instance")]
		public DateTime Instant { get; set; }

		public Period()
		{
			
		}

		public Period(DateTime instant) : this()
		{
			this.Instant = instant;
		}

		#region IEquatable implementation

		public bool Equals(Period other)
		{
			return this.Instant.Equals(other.Instant);
		}

		#endregion
	}
}