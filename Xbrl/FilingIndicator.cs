namespace Xoxo
{
	using System;
	using System.Xml.Serialization;
	using System.Collections.ObjectModel;

	[Serializable]
	[XmlRoot("filingIndicator", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
	public class FilingIndicator : IEquatable<FilingIndicator>
	{
		[XmlAttribute("contextRef")]
		public string ContextId { get; set; }

		[XmlText]
		public string Value { get; set; }

		public FilingIndicator()
		{
		}

		public FilingIndicator(string contextId, string value) : this()
		{
			this.ContextId = contextId;
			this.Value = value;
		}

		#region IEquatable implementation

		public bool Equals(FilingIndicator other)
		{
			return this.Value.Equals(other.Value);
		}

		#endregion
	}

}