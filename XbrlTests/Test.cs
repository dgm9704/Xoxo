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
		static Xbrl CreateSolvencyInstance()
		{
			// Sets default namespaces and units PURE, EUR
			var instance = new Xbrl();

			// Enable some runtime checks
			// When fact is added, checks that the referenced unit exists in the instance
			instance.CheckUnitExists = true;
			// When an explicit member is added, check that the namespace for the domain has been set
			instance.CheckExplicitMemberDomainExists = true;

			// Initialize to the correct framework, module, taxonomy
			// The content is NOT validated against taxonomy or module schema
			// set module
			instance.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");

			// set taxonomy
			instance.TaxonomyVersion = "1.5.2.c";

			// "basic" namespaces
			// These are used for adding correct prefixes for different elements
			instance.SetMetricNamespace("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met");
			instance.SetDimensionNamespace("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
			instance.SetTypedDomainNamespace("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");

			// Namespaces for actual reported values that belong to a domain (explicit members)
			instance.AddDomainNamespace("s2c_CS", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/CS");
			instance.AddDomainNamespace("s2c_AM", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/AM");

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

			// Dimensions and domains can be given with or without namespaces
			// The namespace prefixes are added internally if needed
			// Explicit member values DO NEED the prefix
			scenario.AddExplicitMember("CS", "s2c_CS:x26");
			scenario.AddTypedMember("CE", "ID", "abc");

			// Metrics can also be given with or without namespace
			// Metric names, values or decimals are NOT validated
			// Unit is NOT checked to exist
			instance.AddFact(scenario, "pi545", "uPURE", "4", "0.2547");

			// if a scenario with the given values already exists in the instance, it will be reused
			// you don't have to check for duplicates
			instance.AddFact(scenario, "mi363", "uEUR", "-3", "45345");

			// Non - existing unit throws exception
			try
			{
				instance.AddFact(scenario, "mi363", "uSEK", "-3", "45345");
			}
			catch(InvalidOperationException ex)
			{
				Console.WriteLine(ex.ToString());
			}

			var invalidScenario = new Scenario();

			// Member has value with domain that has no corresponding namespace 
			invalidScenario.AddExplicitMember("CS", "s2c_XA:x0");
			try
			{
				// This can only be observed when the scenario is attached to the instance
				// ie. previous line is ok but this one throws
				instance.AddFact(invalidScenario, "mi252", "uEUR", "4", "123456");
			}
			catch(InvalidOperationException ex)
			{
				Console.WriteLine(ex.ToString());
			}

			return instance;
		}

		[Test]
		public void WriteSolvencyInstance()
		{
			var instance = CreateSolvencyInstance();
			// Write the instace to a file
			var path = @"output.xbrl.xml";
			instance.ToFile(path);
		}

		[Test]
		public void ReadSolvencyReferenceInstance()
		{
			var path = Path.Combine("data", "reference.xbrl.xml");
			var referenceInstance = Xbrl.FromFile(path);
			Assert.IsNotNull(referenceInstance);
		}

		[Test]
		public void CompareSolvencyReferenceInstance()
		{
			var instance = CreateSolvencyInstance(); 

			// They aren't automatically removed until serialization so do it before comparisons
			instance.RemoveUnusedObjects();

			var referencePath = Path.Combine("data", "reference.xbrl.xml");
			var referenceInstance = Xbrl.FromFile(referencePath);

			// Instances are functionally equivalent:
			// They have the same number of contexts and scenarios of the contexts match member-by-member
			// Members are checked by dimension, domain and value, namespaces included
			// They have the same facts matched by metric, value, decimals and unit
			// Entity and Period are also matched
			// Some things are NOT checked, eg. taxonomy version, context names
			Assert.AreEqual(instance, referenceInstance);

			var tempFile = @"temp.xbrl";
			instance.ToFile(tempFile);

			var newInstance = Xbrl.FromFile(tempFile);

			Assert.AreEqual(newInstance, instance);

			Assert.AreEqual(newInstance, referenceInstance);

			newInstance.Contexts[1].AddExplicitMember("AM", "s2c_AM:x1");

			Assert.AreNotEqual(newInstance, referenceInstance);
		}

		[Test]
		public void ReadWriteExampleInstanceArs()
		{
			var inputPath = Path.Combine("data", "ars.xbrl");
			var xbrl = Xbrl.FromFile(inputPath);
			var outputPath = @"output.ars.xbrl";
			xbrl.ToFile(outputPath);
		}
	}
}

