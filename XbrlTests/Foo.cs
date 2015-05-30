using System.Xml.Serialization;
using System.Xml;

namespace XbrlTests
{
	public class Foo
	{
		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces Namespaces;

		[XmlElement]
		public string Name
		{
			get;
			set;
		}

		[XmlElement]
		public XmlQualifiedName QName;

		public Foo()
		{
			var Namespaces = new XmlSerializerNamespaces();
			Namespaces.Add("prefix", "http://www.xbrl.org/2003/instance");
			Namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
			Namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");

			QName = new XmlQualifiedName("foo", "http://www.xbrl.org/2003/instance");
		}
	}
}

