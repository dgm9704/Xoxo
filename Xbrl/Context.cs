namespace Xoxo
{
	using System;
	using System.Xml.Serialization;
	using System.Collections.ObjectModel;

	[Serializable]
	[XmlRoot(ElementName = "context", Namespace = "http://www.xbrl.org/2003/instance")]
	public class Context : IEquatable<Context>
	{

		[XmlAttribute("id", Namespace = "http://www.xbrl.org/2003/instance")]
		public string Id { get; set; }

		[XmlElement("entity", Namespace = "http://www.xbrl.org/2003/instance")]
		public Entity Entity { get; set; }

		[XmlElement("period", Namespace = "http://www.xbrl.org/2003/instance")]
		public Period Period { get; set; }

		[XmlElement("scenario", Namespace = "http://www.xbrl.org/2003/instance")]
		public Scenario Scenario { get; set; }

		public Context()
		{
			this.Scenario = new Scenario();
		}

		public Context(string id) : this()
		{
			this.Id = id;
		}

		#region IEquatable implementation

		public bool Equals(Context other)
		{
			return (this.Entity == null || this.Entity.Equals(other.Entity))
			&& (this.Period == null || this.Period.Equals(other.Period))
			&& this.Scenario.Equals(other.Scenario);
		}

		#endregion
	}

}