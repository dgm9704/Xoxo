namespace Diwen.Xbrl.Tests
{
    using Diwen.Xbrl;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    [TestFixture]
    public class InstanceTests
    {
        static Instance CreateSolvencyInstance()
        {
            // Sets default namespaces and units PURE, EUR
            var instance = new Instance();

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
            instance.Units.Add("uEUR", "iso4217:EUR");
            instance.Units.Add("uPURE", "xbrli:pure");
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

            // Non - existing unit throws KeyNotFoundException
            try
            {
                instance.AddFact(scenario, "mi363", "uSEK", "-3", "45345");
            }
            catch(KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
            }

            return instance;
        }

        [Test]
        public static void WriteSolvencyInstance()
        {
            var instance = CreateSolvencyInstance();
            instance.AddDomainNamespace("s2c_XX", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/XX");
            // Write the instace to a file
            var path = @"output.xbrl.xml";
            instance.ToFile(path);
        }

        [Test]
        public static void ReadSolvencyReferenceInstance()
        {
            var path = Path.Combine("data", "reference.xbrl");
            var referenceInstance = Instance.FromFile(path);
            Assert.IsNotNull(referenceInstance);
        }

        [Test]
        public static void CompareSolvencyReferenceInstance()
        {
            var instance = CreateSolvencyInstance();

            // unless done when loading, duplicate objects 
            // aren't automatically removed until serialization so do it before comparisons
            instance.RemoveUnusedObjects();

            var referencePath = Path.Combine("data", "reference.xbrl");
            var referenceInstance = Instance.FromFile(referencePath, true);

            // Instances are functionally equivalent:
            // They have the same number of contexts and scenarios of the contexts match member-by-member
            // Members are checked by dimension, domain and value, namespaces included
            // They have the same facts matched by metric, value, decimals and unit
            // Entity and Period are also matched
            // Some things are NOT checked, eg. taxonomy version, context names
            //Assert.AreEqual(instance, referenceInstance);

            var tempFile = @"temp.xbrl";
            instance.ToFile(tempFile);

            var newInstance = Instance.FromFile(tempFile, true);

            Assert.IsTrue(newInstance.Equals(instance));

            Assert.IsTrue(newInstance.Equals(referenceInstance));

            newInstance.Contexts[1].AddExplicitMember("AM", "s2c_AM:x1");

            Assert.IsFalse(newInstance.Equals(referenceInstance));
        }

        [Test]
        public static void RoundtripCompareExampleInstanceArs()
        {
            var sw = new Stopwatch();

            var inputPath = Path.Combine("data", "ars.xbrl");

            sw.Start();
            var firstRead = Instance.FromFile(inputPath);
            sw.Stop();
            Console.WriteLine("Read took {0}", sw.Elapsed);

            var outputPath = @"output.ars.xbrl";

            sw.Restart();
            firstRead.ToFile(outputPath);
            sw.Stop();
            Console.WriteLine("Write took {0}", sw.Elapsed);

            sw.Restart();
            var secondRead = Instance.FromFile(outputPath);
            sw.Stop();
            Console.WriteLine("Read took {0}", sw.Elapsed);

            sw.Restart();
            Assert.IsTrue(firstRead.Equals(secondRead));
            sw.Stop();
            Console.WriteLine("Comparison took {0}", sw.Elapsed);
        }

        [Test]
        public static void CompareDimensionOrder()
        {
            var first = new Scenario();
            first.AddExplicitMember("AA", "foo:bar");
            first.AddExplicitMember("BB", "bar:foo");
            first.AddTypedMember("CC", "CA", "aa");
            first.AddTypedMember("DD", "DA", "bb");


            var second = new Scenario();

            second.AddExplicitMember("BB", "bar:foo");
            second.AddExplicitMember("AA", "foo:bar");
            second.AddTypedMember("DD", "DA", "bb");
            second.AddTypedMember("CC", "CA", "aa");

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public static void CollapseDuplicateContexts()
        {
            var inputPath = Path.Combine("data", "duplicate_context.xbrl");
            var instance = Instance.FromFile(inputPath);

            Assert.IsTrue(instance.Contexts.Count == 2);

            instance.CollapseDuplicateContexts();

            Assert.IsTrue(instance.Contexts.Count == 1);
        }

        [Test]
        public static void ReadExampleInstanceFPInd()
        {
            var inputPath = Path.Combine("data", "fp_ind_new_correct.xbrl");
            var first = Instance.FromFile(inputPath);
            Assert.AreEqual(7051, first.Contexts.Count);
            Assert.AreEqual(7091, first.Facts.Count);

            Instance second = null;
            using(var stream = new MemoryStream())
            {
                first.ToStream(stream);
                stream.Seek(0, SeekOrigin.Begin);
                second = Instance.FromStream(stream);
            }

            Assert.AreEqual(first, second);
        }

        [Test]
        public static void RemoveUnusedObjectsPerformance()
        {
            var inputPath = Path.Combine("data", "fp_ind_new_correct.xbrl");
            var xi = Instance.FromFile(inputPath);

            var sw = new Stopwatch();
            sw.Start();
            xi.RemoveUnusedObjects();
            sw.Stop();
            Assert.Less(sw.ElapsedMilliseconds, 2000, "Cleanup takes too long!");
        }


        [Test]
        public static void WriteEmptyTypedMember()
        {
            var instance = CreateSolvencyInstance();
            instance.CheckExplicitMemberDomainExists = true;

            var scenario = new Scenario();
            scenario.AddTypedMember("CE", "ID", null);
            instance.AddFact(scenario, "mi1234", null, null, "123");
            instance.RemoveUnusedObjects();
            instance.ToFile(@"typedmembernil.xbrl.xml");
        }

        [Test]
        public static void WriteToXmlDocument()
        {
            var xmlDoc = CreateSolvencyInstance().ToXmlDocument();
            xmlDoc.Save("xbrl2doc.xml");
        }

        [Test]
        public static void NoMembers()
        {
            var instance = new Instance();
            instance.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");
            instance.TaxonomyVersion = "1.5.2.c";
            instance.SetMetricNamespace("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met");
            instance.SetDimensionNamespace("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
            instance.SetTypedDomainNamespace("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");
            instance.Entity = new Entity("http://standards.iso.org/iso/17442", "00000000000000000098");
            instance.Period = new Period(2015, 12, 31);

            Scenario nullScenario = null;
            instance.AddFact(nullScenario, "foo0", null, null, "alice");

            //Context nullContext = null;
            //instance.AddFact(nullContext, "foo1", null, null, "bob"); // Sorry bob

            var contextWithNullScenario = new Context();
            instance.AddFact(contextWithNullScenario, "foo2", null, null, "carol");

            var contextWithNoMembers = new Context();
            contextWithNoMembers.Scenario = new Scenario();
            instance.AddFact(contextWithNoMembers, "foo3", null, null, "dave");

            var scenarioWithNoMembers = new Scenario();
            instance.AddFact(scenarioWithNoMembers, "foo4", null, null, "erin");

            Assert.AreEqual(1, instance.Contexts.Count);
            Assert.AreEqual(4, instance.Facts.Count);
        }

        [Test]
        public static void FiledOrNot()
        {
            var instance = new Instance();
            instance.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");
            instance.TaxonomyVersion = "1.5.2.c";
            instance.SetMetricNamespace("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met");
            instance.SetDimensionNamespace("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
            instance.SetTypedDomainNamespace("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");
            instance.Entity = new Entity("http://standards.iso.org/iso/17442", "00000000000000000098");
            instance.Period = new Period(2015, 12, 31);

            instance.AddFilingIndicator("foo", true);
            instance.AddFilingIndicator("bar", false);
        }

        [Test]
        public static void EnumeratedFactValueNamespace()
        {
            // Create a minimal test instance
            var instance = new Instance();
            instance.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");
            instance.TaxonomyVersion = "1.5.2.c";
            instance.SetMetricNamespace("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met");
            instance.Entity = new Entity("http://standards.iso.org/iso/17442", "00000000000000000098");
            instance.Period = new Period(2015, 12, 31);

            // add a fact with enumerated value 
            instance.AddFact((Scenario)null, "ei1643", null, null, "s2c_CN:x1");

            // add the namespace for the domain
            instance.AddDomainNamespace("s2c_CN", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/CN");

            // write the instance to file and read it back
            var file = "EnumeratedFactValueNamespace.xbrl";
            instance.ToFile(file);
            instance = Instance.FromFile(file);

            // instance should still contain the namespace for the domain
            Assert.AreEqual("http://eiopa.europa.eu/xbrl/s2c/dict/dom/CN", instance.Namespaces.LookupNamespace("s2c_CN"));
        }

        [Test]
        public static void ReadAndWriteComments()
        {
            // read a test instance with a comment
            var inputPath = Path.Combine("data", "comments.xbrl");
            var xbrl = Instance.FromFile(inputPath);
            Assert.IsTrue(xbrl.Comments.Contains("foo"));

            // add a new comment
            xbrl.Comments.Add("bar");
            var outputPath = Path.Combine("data", "morecomments.xbrl");
            xbrl.ToFile(outputPath);
            xbrl = Instance.FromFile(outputPath);
            Assert.IsTrue(xbrl.Comments.Contains("bar"));
        }
    }
}

