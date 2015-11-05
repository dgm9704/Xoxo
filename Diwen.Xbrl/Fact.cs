namespace Diwen.Xbrl
{
	using System;
	using System.Xml;
	using System.Xml.Serialization;

	public class Fact : IEquatable<Fact>
	{
		private static XmlDocument doc = new XmlDocument();

		[XmlIgnore]
		public Unit Unit { get; set; }

		[XmlIgnore]
		public string Decimals { get; set; }

		[XmlIgnore]
		public Context Context { get; set; }

		[XmlIgnore]
		public XmlQualifiedName Metric { get; set; }

		[XmlIgnore]
		public string Value { get; set; }

		internal string ContextRef;
		internal string UnitRef;

		public Fact()
		{
		}

		public Fact(Context context, string metric, Unit unit, string decimals, string value, string namespaceUri, string prefix)
			: this(context, metric, unit, decimals, value, new Uri(namespaceUri), prefix)
		{
		}

		public Fact(Context context, string metric, Unit unit, string decimals, string value, Uri namespaceUri, string prefix)
		{
			if(context == null)
			{
				throw new ArgumentNullException("context");
			}

			if(namespaceUri == null)
			{
				throw new ArgumentNullException("namespaceUri");
			}

			this.Metric = new XmlQualifiedName(prefix + ":" + metric, namespaceUri.ToString());
			this.Unit = unit;
			this.Decimals = decimals;
			this.Context = context;
			this.Value = value;
		}

		internal XmlElement ToXmlElement()
		{
			var element = doc.CreateElement(this.Metric.Name, this.Metric.Namespace);

			if(this.Context != null)
			{
				element.SetAttribute("contextRef", this.Context.Id);
			}

			if(this.Unit != null)
			{
				element.SetAttribute("unitRef", this.Unit.Id);
			}
			if(!string.IsNullOrEmpty(this.Decimals))
			{
				element.SetAttribute("decimals", this.Decimals);
			}

			element.InnerText = this.Value;
			return element;
		}

		internal static Fact FromXmlElement(XmlElement element)
		{
			var fact = new Fact();
			fact.Metric = new XmlQualifiedName(element.Name, element.NamespaceURI);

			fact.UnitRef = element.GetAttribute("unitRef");
			fact.Decimals = element.GetAttribute("decimals");
			fact.ContextRef = element.GetAttribute("contextRef");
			fact.Value = element.InnerText;
			return fact;
		}

		public override bool Equals(object obj)
		{
			var other = obj as Fact;
			if(other != null)
			{
				return this.Equals(other);
			}
			else
			{
				return base.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			return this.Metric.GetHashCode()
			^ this.Value.GetHashCode()
			^ this.Decimals.GetHashCode();
		}

		#region IEquatable implementation

		public bool Equals(Fact other)
		{
			var result = other != null
			             && this.Metric.Equals(other.Metric)
			             && this.Value.Equals(other.Value, StringComparison.Ordinal)
			             && this.Decimals.Equals(other.Decimals, StringComparison.Ordinal);
			if(result)
			{
				result = this.UnitRef.Equals(other.UnitRef, StringComparison.Ordinal)
				|| this.Unit.Equals(other.Unit);
			}

			return result;
		}

		#endregion
	}
}