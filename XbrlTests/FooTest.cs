//using System;
//using NUnit.Framework;
//using System.Xml.Serialization;
//using System.IO;
//using Xoxo;
//using System.Xml;
//
//namespace XbrlTests
//{
//	[TestFixture]
//	public class FooTest
//	{
//		[Test]
//		public void TestFoo()
//		{
//			var foo = new Foo{ Name = "Bar" };
//
//			var xml = new XmlSerializer(typeof(Foo));
//			//using(var writer = new XbrlWriter(@"foo.output.xml"))
//			using(var writer = XmlWriter.Create(@"foo.output.xml"))
//			{
//				writer.WriteProcessingInstruction("taxonomy-version", "1.5.2.c");
//				xml.Serialize(writer, foo);
//			}
//		}
//	}
//}
//
