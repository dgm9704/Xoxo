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
		public void WriteSolvencyReferenceInstance()
		{
			// Initialize to the correct framework and module
			// Sets any needed namespaces etc
			// Adds default units PURE, EUR
			var instance = XbrlInstance.Create(Framework.SolvencyII, Module.AnnualSolo);

			// Correct taxonomy version has to be set manually
			// The content is NOT validated against taxonomy
			instance.TaxonomyVersion = "1.5.2.c";

			// Add reporter and period
			// These will be reused across all contexts by default
			// Scheme or value are NOT validated
			instance.Entity = new Entity("http://standards.iso.org/iso/17442", "1234567890ABCDEFGHIJ");
			instance.Period = new Period(2014, 12, 31);

			// Any units that aren't used will be excluded during serialization
			// So it's safe to add extra units if needed
			instance.Units.Add("uFOO", "foo:bar");

			// Add filing indicators
			// These are NOT validated against actual reported values
			instance.AddFilingIndicator("S.01.01");
			instance.AddFilingIndicator("S.02.02");

			// A scenario contains the dimensions and their values for a datapoint
			var scenario = new Scenario();

			// dimensions and domains can be given with or without namespaces
			// the namespaces are added internally if needed
			scenario.AddExplicitMember("CS", "s2c_CS:x26");
			scenario.AddTypedMember("CE", "ID", "abc");

			// Metrics can also be given with or without namespace
			// Metric names, values or decimals are NOT validated
			// Unit is NOT checked to exist
			instance.AddFact(scenario, "pi545", "uPURE", "4", "0.2547");

			// if a scenario with the given values already exists in the instance, it will be reused
			// you don't have to check for dupicates
			instance.AddFact(scenario, "mi363", "uEUR", "-3", "45345");

			// Write the instace to a file
			var path = @"output.xbrl.xml";
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

