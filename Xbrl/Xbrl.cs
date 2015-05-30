namespace Xoxo
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.Serialization;
	using System.IO;
	using System.Linq;
	using System.Collections.ObjectModel;
	using System.Text;

	[Serializable]
	[XmlRoot(ElementName = "xbrl", Namespace = "http://www.xbrl.org/2003/instance")]
	public class Xbrl : IEquatable<Xbrl>
	{
		[XmlIgnore]		
		public XmlNamespaceManager Namespaces { get; set; }

		[XmlIgnore]
		public Entity Entity { get; set; }

		[XmlIgnore]
		public Period Period { get; set; }

		[XmlIgnore]
		public string TaxonomyVersion { get; set; }

		[XmlIgnore]
		public string FactNamespace { get; private set ; }

		[XmlIgnore]
		public string DimensionNamespace { get; private set; }

		[XmlIgnore]
		public string TypedDomainNamespace { get; private set; }

		[XmlElement("schemaRef", Namespace = "http://www.xbrl.org/2003/linkbase")]
		public SchemaReference SchemaReference { get; set; }

		[XmlIgnore]
		public UnitCollection Units { get; set; }

		[XmlElement("unit", Namespace = "http://www.xbrl.org/2003/instance")]
		public UnitCollection UsedUnit { get { return Units.UsedUnits(); } set { throw new NotImplementedException(); } }

		[XmlArray("fIndicators", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
		[XmlArrayItem("filingIndicator", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
		public FilingIndicatorCollection FilingIndicators { get; set; }

		[XmlElement("context", Namespace = "http://www.xbrl.org/2003/instance")]
		public ContextCollection Contexts { get; set; }

		[XmlIgnore]
		public FactCollection Facts { get; set; }

		[XmlAnyElement]
		public XmlElement[] FactItems
		{
			get
			{
				var elements = new List<XmlElement>();
				foreach (var item in Facts)
				{
					elements.Add(item.ToXmlElement());
				}
				return elements.ToArray();
			}
			set
			{ 
				foreach (var element in value)
				{
					this.Facts.Add(Fact.FromXmlElement(element));
				}
			}
		}

		[XmlIgnore]
		public bool CheckUnitExists	{ get; set; }

		[XmlIgnore]
		public bool CheckExplicitMemberDomainExists	{ get; set; }

		public void SetDimensionNamespace(string prefix, string namespaceUri)
		{
			this.Namespaces.AddNamespace(prefix, namespaceUri);
			this.DimensionNamespace = namespaceUri;
		}

		public void SetMetricNamespace(string prefix, string namespaceUri)
		{
			this.Namespaces.AddNamespace(prefix, namespaceUri);
			this.FactNamespace = namespaceUri;
		}

		public void SetTypedDomainNamespace(string prefix, string namespaceUri)
		{
			this.Namespaces.AddNamespace(prefix, namespaceUri);
			this.TypedDomainNamespace = namespaceUri;
		}

		public void AddDomainNamespace(string prefix, string namespaceUri)
		{
			this.Namespaces.AddNamespace(prefix, namespaceUri);
		}

		public Context GetContext(Scenario scenario)
		{
			Context context = null;

			if (scenario == null)
			{
				context = this.Contexts.FirstOrDefault(c => c.Scenario == null);
			}
			else
			{
				context = this.Contexts.FirstOrDefault(c => scenario.Equals(c.Scenario));
			}
			 
			if (context == null)
			{
				context = new Context(scenario);
				Contexts.Add(context);
			}

			return context;
		}

		private void AddDefaultNamespaces()
		{
			Namespaces.AddNamespace("xbrli", "http://www.xbrl.org/2003/instance");
			Namespaces.AddNamespace("link", "http://www.xbrl.org/2003/linkbase");
			Namespaces.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
			Namespaces.AddNamespace("iso4217", "http://www.xbrl.org/2003/iso4217");
			Namespaces.AddNamespace("find", "http://www.eurofiling.info/xbrl/ext/filing-indicators");
			Namespaces.AddNamespace("xbrldi", "http://xbrl.org/2006/xbrldi");
		}

		public void RemoveUnusedObjects()
		{
			var used = Units.UsedUnits();
			this.Units.Clear();
			this.Units.AddRange(used);
		}

		public Xbrl()
		{
			this.Namespaces = new XmlNamespaceManager(new NameTable());
			AddDefaultNamespaces();
			this.SchemaReference = new SchemaReference();
			this.FilingIndicators = new FilingIndicatorCollection(this);
			this.Units = new UnitCollection(this); 
			this.Contexts = new ContextCollection(this);
			this.Facts = new FactCollection(this);
		}

		#region IEquatable implementation

		public bool Equals(Xbrl other)
		{
			var result = false;

			if (this.SchemaReference.Equals(other.SchemaReference))
			{
				if (this.Units.Equals(other.Units))
				{
					if (this.FilingIndicators.Equals(other.FilingIndicators))
					{
						if (this.Contexts.Equals(other.Contexts))
						{
							if (this.Facts.Equals(other.Facts))
							{
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		#endregion


		private static XmlSerializer Serializer = new XmlSerializer(typeof(Xbrl));

		public static Xbrl FromFile(string path)
		{
			Xbrl xbrl = null;

			using (var inputfile = new FileStream(path, FileMode.Open))
			{
				xbrl = (Xbrl)Serializer.Deserialize(inputfile);
			}
			return xbrl;
		}

		public void ToFile(string path)
		{

			var xmlns = this.Namespaces.ToXmlSerializerNamespaces();

			var settings = new XmlWriterSettings
			{
				Indent = true,
				NamespaceHandling = NamespaceHandling.OmitDuplicates,
				Encoding = UTF8Encoding.UTF8
			};
			using (var writer = XmlWriter.Create(path, settings))
			{
				if (!string.IsNullOrEmpty(this.TaxonomyVersion))
				{
					writer.WriteProcessingInstruction("taxonomy-version", this.TaxonomyVersion);
				}

				Serializer.Serialize(writer, this, xmlns);
			}
		}
	}
}