namespace XbrlTests
{
	using System;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Xml;
	using System.Xml.Serialization;
	using NUnit.Framework;
	using Xoxo;

	[TestFixture]
	public class Test
	{
		[Test]
		public void WriteReferenceInstance()
		{
			var instance = XbrlInstance.Create(Framework.SolvencyII, Module.AnnualSolo);

			instance.Entity = new Entity("http://standards.iso.org/iso/17442", "1234567890ABCDEFGHIJ");
			instance.Period = new Period(2014, 12, 31);

			instance.AddFilingIndicator("S.01.01.01");
			instance.AddFilingIndicator("S.02.02.01");

			var scenario = new Scenario();

			scenario.AddExplicitMember("CS", "s2c_CS:x26");
			scenario.AddTypedMember("CE", "ID", "abc");

			instance.AddFact(scenario, "pi545", "uPURE", "4", "0.2547");
			instance.AddFact(scenario, "mi363", "uEUR", "-3", "45345");

			var path = @"output.xbrl";
			instance.ToFile(path);
		}

		[Test]
		public void ReadReferenceInstance()
		{
			var path = Path.Combine("data", "reference.xbrl");
			var xbrl = XbrlInstance.FromFile(path);
			Assert.IsNotNull(xbrl);
		}

		[Test]
		public void ReadWriteExampleInstanceArs()
		{
			var inputPath = Path.Combine("data", "ars.xbrl");
			var xbrl = XbrlInstance.FromFile(inputPath);
			var outputPath = @"output.ars.xbrl";
			xbrl.ToFile(outputPath);
		}
	}
}

