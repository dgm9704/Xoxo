using System;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using Xoxo;
using System.Xml;

namespace XbrlTests
{
	[TestFixture]
	public class FooTest
	{
		[Test]
		public void TestFoo()
		{
			var foo = new Foo{ Name = "Bar" };
			var Serializer = new XmlSerializer(typeof(Foo));
			var xmlns = new XmlSerializerNamespaces();
			xmlns.Add("prefix", "http://www.xbrl.org/2003/instance");

			var settings = new XmlWriterSettings{ Indent = true }; //, NamespaceHandling = NamespaceHandling.OmitDuplicates };
			using(var writer = XmlWriter.Create(@"foo.output.xml", settings))
			{
				writer.WriteProcessingInstruction("taxonomy-version", "1.5.2.c");
				Serializer.Serialize(writer, foo);//, xmlns);
			}
		}

		[Test]
		public void QualifiedNameTest()
		{
			//var qualifiedName = new XmlQualifiedName("foo", "http://www.xbrl.org/2003/instance");

		}
	}
}

