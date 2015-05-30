using System.Xml;

namespace XbrlTests
{
	public class TestClass
	{
		[XmlElement]
		public string Name
		{
			get;
			set;
		}

		public TestClass()
		{
		}
	}
}

