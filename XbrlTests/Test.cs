namespace XbrlTests
{
	using System;
	using System.IO;
	using System.Xml.Serialization;
	using NUnit.Framework;
	using Xoxo;

	[TestFixture]
	public class Test
	{
		[Test]
		public void WriteReferenceInstance()
		{
			var xbrl = new XbrlInstance();

			xbrl.ContextEntity = new Entity("http://standards.iso.org/iso/17442", "1234567890ABCDEFGHIJ");
			xbrl.ContextPeriod = new Period(new DateTime(2014, 12, 31));
			xbrl.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");
			xbrl.FactNamespace = "http://eiopa.europa.eu/xbrl/s2md/dict/met";

			var context = xbrl.Contexts.Add();
			xbrl.FilingIndicators.Add(context.Id, "S.01.01.01");
			xbrl.FilingIndicators.Add(context.Id, "S.02.02.01");

			context = new Context();

			context.Scenario.ExplicitMembers.Add("eba_dim:CS", "s2c_CS:x26");
			context.Scenario.TypedMembers.Add("eba_dim:CE", "s2c_typ:ID", "abc");

			var id = xbrl.Contexts.Add(context);

			xbrl.Facts.Add("s2md_met:pi545", "uPURE", "4", id, "0.2547");
			xbrl.Facts.Add("s2md_met:mi363", "uEUR", "-3", id, "45345");

			var newContext = new Context();

			newContext.Scenario.ExplicitMembers.Add("eba_dim:CS", "s2c_CS:x26");
			newContext.Scenario.TypedMembers.Add("eba_dim:CE", "s2c_typ:ID", "abc");

			id = xbrl.Contexts.Add(context);
			var path = @"output.xbrl";
			xbrl.ToFile(path);
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

