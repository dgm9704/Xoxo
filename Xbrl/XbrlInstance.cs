namespace Xoxo
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.Serialization;
	using System.IO;
	using System.Linq;
	using System.Collections.ObjectModel;

	[Serializable]
	[XmlRoot(ElementName = "xbrl", Namespace = "http://www.xbrl.org/2003/instance")]
	public class XbrlInstance : IEquatable<XbrlInstance>
	{
		[XmlIgnore]
		public Entity Entity { get; set; }

		[XmlIgnore]
		public Period Period { get; set; }

		[XmlIgnore]
		public string FactNamespace { get; set; }

		[XmlIgnore]
		public string FactPrefix { get; set; }

		[XmlIgnore]
		public string DimensionPrefix { get; set; }

		[XmlIgnore]
		public string TypedDomainPrefix { get; set; }

		[XmlIgnore]
		public string TaxonomyVersion { get; set; }

		[XmlElement("schemaRef", Namespace = "http://www.xbrl.org/2003/linkbase")]
		public SchemaReference SchemaReference { get; set; }

		[XmlIgnore]
		public UnitCollection Units { get; set; }

		[XmlElement("unit", Namespace = "http://www.xbrl.org/2003/instance")]
		public Collection<Unit> UsedUnit { get { return Units.UsedUnits(); } set { throw new NotImplementedException(); } }

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
				foreach(var item in Facts)
				{
					elements.Add(item.ToXmlElement());
				}
				return elements.ToArray();
			}
			set
			{ 
				foreach(var element in value)
				{
					this.Facts.Add(Fact.FromXmlElement(element));
				}
			}
		}

		public Context GetContext(Scenario scenario)
		{
			Context context = null;

			if(scenario == null)
			{
				context = this.Contexts.FirstOrDefault(c => c.Scenario == null);
			}
			else
			{
				context = this.Contexts.FirstOrDefault(c => scenario.Equals(c.Scenario));
			}
			 
			if(context == null)
			{
				context = new Context(scenario);
				Contexts.Add(context);
			}

			return context;
		}

		public List<XmlQualifiedName> GetDefaultNamespaces()
		{
			var xmlns = new List<XmlQualifiedName>();
			xmlns.Add(new XmlQualifiedName("xbrli", "http://www.xbrl.org/2003/instance"));
			xmlns.Add(new XmlQualifiedName("link", "http://www.xbrl.org/2003/linkbase"));
			xmlns.Add(new XmlQualifiedName("xlink", "http://www.w3.org/1999/xlink"));
			xmlns.Add(new XmlQualifiedName("iso4217", "http://www.xbrl.org/2003/iso4217"));
			xmlns.Add(new XmlQualifiedName("find", "http://www.eurofiling.info/xbrl/ext/filing-indicators"));
			xmlns.Add(new XmlQualifiedName("xbrldi", "http://xbrl.org/2006/xbrldi"));
			return xmlns;
		}

		[XmlIgnore]
		public List<XmlQualifiedName> Namespaces;

		public XbrlInstance()
		{
			this.Namespaces = GetDefaultNamespaces();
			this.SchemaReference = new SchemaReference();
			this.FilingIndicators = new FilingIndicatorCollection(this);
			this.Units = new UnitCollection(this); 
			this.Contexts = new ContextCollection(this);
			this.Facts = new FactCollection(this);
		}

		#region IEquatable implementation

		public bool Equals(XbrlInstance other)
		{
			return this.SchemaReference.Equals(other.SchemaReference)
			&& this.Units.Equals(other.Units)
			&& this.FilingIndicators.Equals(other.FilingIndicators)
			&& this.Contexts.Equals(other.Contexts)
			&& this.Facts.Equals(other.Facts);
		}

		#endregion


		private static XmlSerializer Serializer = new XmlSerializer(typeof(XbrlInstance));

		public static XbrlInstance FromFile(string path)
		{
			XbrlInstance xbrl = null;

			using(var inputfile = new FileStream(path, FileMode.Open))
			{
				xbrl = (XbrlInstance)Serializer.Deserialize(inputfile);
			}
			return xbrl;
		}

		public void ToFile(string path)
		{
			var xmlns = new XmlSerializerNamespaces(this.Namespaces.ToArray());
			var settings = new XmlWriterSettings{ Indent = true, NamespaceHandling = NamespaceHandling.OmitDuplicates };
			using(var writer = XmlWriter.Create(path, settings))
			{
				writer.WriteProcessingInstruction("taxonomy-version", this.TaxonomyVersion);
				Serializer.Serialize(writer, this, xmlns);
			}
		}

		public static XbrlInstance Create(Framework framework, Module module)
		{
			var xbrl = new XbrlInstance();

			switch(framework)
			{
			case Framework.SolvencyII:
				xbrl.Namespaces.AddRange(SolvencyIINamespaces());
				xbrl.FactNamespace = "http://eiopa.europa.eu/xbrl/s2md/dict/met";
				xbrl.FactPrefix = "s2md_met";
				xbrl.DimensionPrefix = "s2c_dim";
				xbrl.TypedDomainPrefix = "s2c_typ";

				switch(module)
				{
				case Module.AnnualSolo:
					xbrl.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");
					break;
				default:
					throw new NotSupportedException("Module not yet supported");
				}
				break;

			default:
				throw new NotSupportedException("Framework not yet supported");
			}

			return xbrl;
		}

		public static Collection<XmlQualifiedName> SolvencyIINamespaces()
		{
			var xmlns = new Collection<XmlQualifiedName>();
			xmlns.Add(new XmlQualifiedName("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim"));
			xmlns.Add(new XmlQualifiedName("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ"));
			xmlns.Add(new XmlQualifiedName("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met"));
			xmlns.Add(new XmlQualifiedName("s2c_CS", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/CS"));
			xmlns.Add(new XmlQualifiedName("s2c_CU", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/CU"));
			xmlns.Add(new XmlQualifiedName("s2c_AM", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/AM"));
			xmlns.Add(new XmlQualifiedName("s2c_SE", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/SE"));
			xmlns.Add(new XmlQualifiedName("s2c_AP", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/AP"));
			xmlns.Add(new XmlQualifiedName("s2c_PU", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/PU"));
			xmlns.Add(new XmlQualifiedName("s2c_GA", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/GA"));
			return xmlns;
		}
	}
}