namespace Xoxo
{
	using System;
	using System.Collections.ObjectModel;
	using System.Xml;

	public class XbrlHelper
	{
		public static XbrlInstance CreateInstance(Framework framework, Module module)
		{
			var xbrl = new XbrlInstance();
			xbrl.Namespaces.AddRange(SolvencyIINamespaces());

			xbrl.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");
			xbrl.FactNamespace = "http://eiopa.europa.eu/xbrl/s2md/dict/met";
			xbrl.FactPrefix = "s2md_met";
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

