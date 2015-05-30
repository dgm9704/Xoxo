using System.IO;

namespace Xoxo
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.Serialization;

	[Serializable]
	[XmlRoot(ElementName = "xbrl", Namespace = "http://www.xbrl.org/2003/instance")]
	public class XbrlInstance : IEquatable<XbrlInstance>
	{
		[XmlIgnore]
		public Entity ContextEntity		{ get; set; }

		[XmlIgnore]
		public Period ContextPeriod		{ get; set; }

		[XmlIgnore]
		public string FactNamespace		{ get; set; }

		[XmlIgnore]
		public string TaxonomyVersion { get; set; }

		private XmlDocument doc = new XmlDocument();
		private XmlProcessingInstruction processingInstruction;

		[XmlAnyElement]
		public XmlProcessingInstruction[] ProcessingInstruction
		{
			get
			{
				var result = new List<XmlProcessingInstruction>();
				if(processingInstruction == null)
				{
					processingInstruction = doc.CreateProcessingInstruction("taxonomy-version", this.TaxonomyVersion);
				}
				result.Add(processingInstruction);
				return result.ToArray();
			}
		}

		[XmlElement("schemaRef", Namespace = "http://www.xbrl.org/2003/linkbase")]
		public SchemaReference SchemaReference { get; set; }

		[XmlElement("unit", Namespace = "http://www.xbrl.org/2003/instance")]
		public UnitCollection Units { get; set; }

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

		public XbrlInstance()
		{
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

		public static XmlSerializerNamespaces S2Namespaces()
		{
			var xmlns = new XmlSerializerNamespaces();
			xmlns.Add("xbrli", "http://www.xbrl.org/2003/instance");
			xmlns.Add("link", "http://www.xbrl.org/2003/linkbase");
			xmlns.Add("xlink", "http://www.w3.org/1999/xlink");
			xmlns.Add("iso4217", "http://www.xbrl.org/2003/iso4217");
			xmlns.Add("find", "http://www.eurofiling.info/xbrl/ext/filing-indicators");
			xmlns.Add("xbrldi", "http://xbrl.org/2006/xbrldi");
			xmlns.Add("s2c_CS", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/CS");
			xmlns.Add("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
			xmlns.Add("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");
			xmlns.Add("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met");
			xmlns.Add("s2c_CU", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/CU");
			xmlns.Add("s2c_AM", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/AM");
			xmlns.Add("s2c_SE", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/SE");
			xmlns.Add("s2c_AP", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/AP");
			xmlns.Add("s2c_PU", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/PU");
			xmlns.Add("s2c_GA", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/GA");
			return xmlns;
		}

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
			var xmlns = XbrlInstance.S2Namespaces();
			using(var outputFile = new FileStream(path, FileMode.Create))
			{
				Serializer.Serialize(outputFile, this, xmlns);
			}
		}
	}
}