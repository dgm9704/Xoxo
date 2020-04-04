//
//  InstanceTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2020 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Xunit;
    using Xbrl;

    public static class InstanceTests
    {
        internal static Instance CreateSolvencyInstance()
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
            catch (KeyNotFoundException ex)
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
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return instance;
        }

        [Fact]
        public static void WriteSolvencyInstance()
        {
            var instance = CreateSolvencyInstance();
            instance.AddDomainNamespace("s2c_XX", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/XX");
            // Write the instace to a file
            string path = "output.xbrl.xml";
            instance.ToFile(path);
        }

        [Fact]
        public static void ReadSolvencyReferenceInstance()
        {
            var path = Path.Combine("data", "reference.xbrl");
            var referenceInstance = Instance.FromFile(path);
            Assert.NotNull(referenceInstance);
        }

        [Fact]
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
            //Assert.Equal(instance, referenceInstance);

            string tempFile = "temp.xbrl";
            instance.ToFile(tempFile);

            var newInstance = Instance.FromFile(tempFile, true);

            // Assert.True(newInstance.Equals(instance));
            var report = InstanceComparer.Report(instance, newInstance);
            Assert.Empty(report.Messages);

            Assert.True(newInstance.Equals(referenceInstance));

            newInstance.Contexts[1].AddExplicitMember("AM", "s2c_AM:x1");

            Assert.False(newInstance.Equals(referenceInstance));
        }

        [Fact]
        public static void RoundtripCompareExampleInstanceArs()
        {
            var sw = new Stopwatch();

            var inputPath = Path.Combine("data", "ars.xbrl");

            sw.Start();
            var firstRead = Instance.FromFile(inputPath);
            sw.Stop();
            Console.WriteLine("Read took {0}", sw.Elapsed);

            var outputPath = "output.ars.xbrl";

            sw.Restart();
            firstRead.ToFile(outputPath);
            sw.Stop();
            Console.WriteLine("Write took {0}", sw.Elapsed);

            sw.Restart();
            var secondRead = Instance.FromFile(outputPath);
            sw.Stop();
            Console.WriteLine("Read took {0}", sw.Elapsed);

            sw.Restart();
            Assert.True(firstRead.Equals(secondRead));
            sw.Stop();
            Console.WriteLine("Comparison took {0}", sw.Elapsed);
        }

        [Fact]
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

            Assert.True(first.Equals(second));
        }

        [Fact]
        public static void CollapseDuplicateContexts()
        {
            var inputPath = Path.Combine("data", "duplicate_context.xbrl");
            Instance instance = null;
            using (var stream = new FileStream(inputPath, FileMode.Open))
                instance = Instance.FromStream(stream, removeUnusedObjects: false, collapseDuplicateContexts: false);

            Assert.Equal(2, instance.Contexts.Count);

            instance.CollapseDuplicateContexts();
            Assert.Single(instance.Contexts);

            instance = Instance.FromFile(inputPath);
            Assert.Single(instance.Contexts);
        }

        [Fact]
        public static void ReadExampleInstanceFPInd()
        {
            var inputPath = Path.Combine("data", "fp_ind_new_correct.xbrl");
            var first = Instance.FromFile(inputPath);
            Assert.Equal(7051, first.Contexts.Count);
            Assert.Equal(7091, first.Facts.Count);

            Instance second;
            using (var stream = new MemoryStream())
            {
                first.ToStream(stream);
                stream.Seek(0, SeekOrigin.Begin);
                second = Instance.FromStream(stream);
            }

            Assert.Equal(first, second);
        }

        [Fact]
        public static void RemoveUnusedObjectsPerformance()
        {
            var inputPath = Path.Combine("data", "fp_ind_new_correct.xbrl");
            var xi = Instance.FromFile(inputPath);

            var sw = new Stopwatch();
            sw.Start();
            xi.RemoveUnusedObjects();
            sw.Stop();
            Assert.False(sw.ElapsedMilliseconds > 2000, "Cleanup takes too long!");
        }

        [Fact]
        public static void WriteEmptyTypedMember()
        {
            var instance = CreateSolvencyInstance();
            instance.CheckExplicitMemberDomainExists = true;

            var scenario = new Scenario();
            scenario.AddTypedMember("CE", "ID", null);
            instance.AddFact(scenario, "mi1234", null, null, "123");
            instance.RemoveUnusedObjects();
            instance.ToFile("typedmembernil.xbrl.xml");
        }

        [Fact]
        public static void WriteToXmlDocument()
        => CreateSolvencyInstance().
                                   ToXmlDocument().
                                   Save("xbrl2doc.xml");

        [Fact]
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

            Assert.Single(instance.Contexts);
            Assert.Equal(4, instance.Facts.Count);
        }

        [Fact]
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

        [Fact]
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
            string file = "EnumeratedFactValueNamespace.xbrl";
            instance.ToFile(file);
            instance = Instance.FromFile(file);

            // instance should still contain the namespace for the domain
            Assert.Equal("http://eiopa.europa.eu/xbrl/s2c/dict/dom/CN", instance.Namespaces.LookupNamespace("s2c_CN"));
        }

        [Fact]
        public static void ReadAndWriteComments()
        {
            // read a test instance with a comment
            var inputPath = Path.Combine("data", "comments.xbrl");
            var xbrl = Instance.FromFile(inputPath);
            Assert.Contains("foo", xbrl.Comments);

            // add a new comment
            xbrl.Comments.Add("bar");
            var outputPath = Path.Combine("data", "morecomments.xbrl");
            xbrl.ToFile(outputPath);
            xbrl = Instance.FromFile(outputPath);
            Assert.Contains("bar", xbrl.Comments);
        }

        [Fact]
        public static void NoExtraNamespaces()
        {
            var instance = Instance.FromFile(Path.Combine("data", "comments.xbrl"));
            instance.SetDimensionNamespace("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
            instance.SetTypedDomainNamespace("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");
            instance.ToFile("ns.out");
        }

        [Fact]
        public static void EmptyInstance()
        {
            // should load ok
            var instance = Instance.FromFile(Path.Combine("data", "empty_instance.xbrl"));
            Assert.NotNull(instance);
            instance.ToFile("empty_instance_out.xbrl");
        }

        [Fact]
        public static void InstanceFromString()
        {
            var input = File.ReadAllText(Path.Combine("data", "comments.xbrl"));
            var instance = Instance.FromXml(input);
            var output = instance.ToXml();
            Assert.NotEmpty(output);
            // Most probably wont't match due to differences in casing or apostrophe vs. quotation etc.
            // // Assert.Equal(input, output);
        }

        [Fact]
        public static void SerializedInstanceWithNoMonetaryUnitShouldNotHaveUnusedNamespace()
        {
            var inFile = Path.Combine("data", "minimal.xbrl");
            var instance = Instance.FromFile(inFile);
            var outFile = "minimal.out";
            instance.ToFile(outFile);
            var filecontent = File.ReadAllText(outFile);
            Assert.DoesNotContain("iso4217", filecontent);
        }

		[Fact]
		public static void ExplicitMembersWithSurroundingWhitespaceShouldNotBork()
		{
			var infile = Path.Combine("data", "example_erst_dcca.xbrl");
			var instance = Instance.FromFile(infile);
			Assert.NotNull(instance);	
		}

    }
}
