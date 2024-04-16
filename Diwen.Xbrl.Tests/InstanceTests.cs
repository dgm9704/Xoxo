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
    using Xunit.Abstractions;
    using Diwen.Xbrl.Xml.Comparison;
    using Diwen.Xbrl.Xml;

    public class InstanceTests
    {

        private readonly ITestOutputHelper output;

        public InstanceTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        internal Report CreateSolvencyInstance()
        {
            // Sets default namespaces and units PURE, EUR
            var instance = new Report();

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
                output.WriteLine(ex.Message);
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
                output.WriteLine(ex.Message);
            }

            return instance;
        }

        [Fact]
        public void WriteSolvencyInstance()
        {
            var instance = CreateSolvencyInstance();
            instance.AddDomainNamespace("s2c_XX", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/XX");
            // Write the instace to a file
            string path = "output.xbrl.xml";
            instance.ToFile(path);
        }

        [Fact]
        public void ReadSolvencyReferenceInstance()
        {
            var path = Path.Combine("data", "reference.xbrl");
            var referenceInstance = Report.FromFile(path);
            Assert.NotNull(referenceInstance);
        }

        [Fact]
        public void CompareSolvencyReferenceInstance()
        {
            var instance = CreateSolvencyInstance();

            // unless done when loading, duplicate objects 
            // aren't automatically removed until serialization so do it before comparisons
            instance.RemoveUnusedObjects();

            var referencePath = Path.Combine("data", "reference.xbrl");
            var referenceInstance = Report.FromFile(referencePath);

            // Instances are functionally equivalent:
            // They have the same number of contexts and scenarios of the contexts match member-by-member
            // Members are checked by dimension, domain and value, namespaces included
            // They have the same facts matched by metric, value, decimals and unit
            // Entity and Period are also matched
            // Some things are NOT checked, eg. taxonomy version, context names
            //Assert.Equal(instance, referenceInstance);

            string tempFile = "temp.xbrl";
            instance.ToFile(tempFile);

            var newInstance = Report.FromFile(tempFile);

            // Assert.True(newInstance.Equals(instance));
            var report = ReportComparer.Report(instance, newInstance);
            if (!report.Result)
                Console.WriteLine(report);
            Assert.Empty(report.Messages);

            Assert.True(newInstance.Equals(referenceInstance));

            newInstance.Contexts[1].AddExplicitMember("AM", "s2c_AM:x1");

            Assert.False(newInstance.Equals(referenceInstance));
        }

        [Fact]
        public void RoundtripCompareExampleInstanceArs()
        {
            var sw = new Stopwatch();

            var inputPath = Path.Combine("data", "ars.xbrl");

            sw.Start();
            var firstRead = Report.FromFile(inputPath);
            sw.Stop();
            output.WriteLine("Read took {0}", sw.Elapsed);

            var outputPath = "output.ars.xbrl";

            sw.Restart();
            firstRead.ToFile(outputPath);
            sw.Stop();
            output.WriteLine("Write took {0}", sw.Elapsed);

            sw.Restart();
            var secondRead = Report.FromFile(outputPath);
            sw.Stop();
            output.WriteLine("Read took {0}", sw.Elapsed);

            sw.Restart();
            Assert.True(firstRead.Equals(secondRead));
            sw.Stop();
            output.WriteLine("Comparison took {0}", sw.Elapsed);
        }

        [Fact]
        public void CompareDimensionOrder()
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
        public void CollapseDuplicateContexts()
        {
            var inputPath = Path.Combine("data", "duplicate_context.xbrl");
            Report instance = null;
            using (var stream = new FileStream(inputPath, FileMode.Open))
                instance = Report.FromStream(stream, removeUnusedObjects: false, collapseDuplicateContexts: false, removeDuplicateFacts: false);

            Assert.Equal(2, instance.Contexts.Count);

            instance.CollapseDuplicateContexts();
            Assert.Single(instance.Contexts);

            instance = Report.FromFile(inputPath);
            Assert.Single(instance.Contexts);
        }

        [Fact]
        public void ReadExampleInstanceFPInd()
        {
            var inputPath = Path.Combine("data", "fp_ind_new_correct.xbrl");
            var first = Report.FromFile(inputPath, removeUnusedObjects: false, collapseDuplicateContexts: false, removeDuplicateFacts: false);
            Assert.Equal(7051, first.Contexts.Count);
            Assert.Equal(7091, first.Facts.Count);

            Report second;
            using (var stream = new MemoryStream())
            {
                first.ToStream(stream);
                stream.Seek(0, SeekOrigin.Begin);
                second = Report.FromStream(stream, removeUnusedObjects: false, collapseDuplicateContexts: false, removeDuplicateFacts: false);
            }

            var report = ReportComparer.Report(first, second);
            Assert.True(report.Result, string.Join(Environment.NewLine, report.Messages));
        }

        [Fact]
        public void RemoveUnusedObjectsPerformance()
        {
            var inputPath = Path.Combine("data", "fp_ind_new_correct.xbrl");
            var xi = Report.FromFile(inputPath);

            var sw = new Stopwatch();
            sw.Start();
            xi.RemoveUnusedObjects();
            sw.Stop();
            Assert.False(sw.ElapsedMilliseconds > 2000, "Cleanup takes too long!");
        }

        [Fact]
        public void WriteEmptyTypedMember()
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
        public void WriteToXmlDocument()
        => CreateSolvencyInstance().
                                   ToXmlDocument().
                                   Save("xbrl2doc.xml");

        [Fact]
        public void NoMembers()
        {
            var instance = new Report();
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
        public void FiledOrNot()
        {
            var instance = new Report();
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
        public void EnumeratedFactValueNamespace()
        {
            // Create a minimal test instance
            var instance = new Report();
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
            instance = Report.FromFile(file);

            // instance should still contain the namespace for the domain
            Assert.Equal("http://eiopa.europa.eu/xbrl/s2c/dict/dom/CN", instance.Namespaces.LookupNamespace("s2c_CN"));
        }

        [Fact]
        public void ReadAndWriteComments()
        {
            // read a test instance with a comment
            var inputPath = Path.Combine("data", "comments.xbrl");
            var xbrl = Report.FromFile(inputPath);
            Assert.Contains("foo", xbrl.Comments);

            // add a new comment
            xbrl.Comments.Add("bar");
            var outputPath = Path.Combine("data", "morecomments.xbrl");
            xbrl.ToFile(outputPath);
            xbrl = Report.FromFile(outputPath);
            Assert.Contains("bar", xbrl.Comments);
        }

        [Fact]
        public void NoExtraNamespaces()
        {
            var instance = Report.FromFile(Path.Combine("data", "comments.xbrl"));
            instance.SetDimensionNamespace("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
            instance.SetTypedDomainNamespace("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");
            instance.ToFile("ns.out");
        }

        [Fact]
        public void EmptyInstance()
        {
            // should load ok
            var instance = Report.FromFile(Path.Combine("data", "empty_instance.xbrl"));
            Assert.NotNull(instance);
            instance.ToFile("empty_instance_out.xbrl");
        }

        [Fact]
        public void InstanceFromString()
        {
            var input = File.ReadAllText(Path.Combine("data", "comments.xbrl"));
            var instance = Report.FromXml(input);
            var output = instance.ToXml();
            Assert.NotEmpty(output);
            // Most probably wont't match due to differences in casing or apostrophe vs. quotation etc.
            // Assert.Equal(input, output);
        }

        [Fact]
        public void SerializedInstanceWithNoMonetaryUnitShouldNotHaveUnusedNamespace()
        {
            var inFile = Path.Combine("data", "minimal.xbrl");
            var instance = Report.FromFile(inFile);
            var outFile = "minimal.out";
            instance.ToFile(outFile);
            var filecontent = File.ReadAllText(outFile);
            Assert.DoesNotContain("iso4217", filecontent);
        }

        [Fact]
        public void ExplicitMembersWithSurroundingWhitespaceShouldNotBork()
        {
            var infile = Path.Combine("data", "example_erst_dcca.xbrl");
            var instance = Report.FromFile(infile);
            Assert.NotNull(instance);
        }

        [Fact]
        public void FactWithNullContextShouldNotThrow_71()
        {
            var infile = Path.Combine("data", "71.xbrl");
            var instance = Report.FromFile(infile);
            Assert.NotNull(instance);
        }

        [Fact]
        public void ExplicitMemberNamespaceOverwriting_72()
        {
            var instance = new Report();
            var intermediaryEntity = new Entity("http://www.ato.gov.au/abn", "123456789") { Report = instance };
            instance.SetDimensionNamespace("h04", "http://sbr.gov.au/dims/RprtPyType.02.00.dims");
            intermediaryEntity.AddExplicitMember("ReportPartyTypeDimension", "h04:Intermediary");
            instance.SetDimensionNamespace("h05", "http://sbr.gov.au/dims/TaxOblgtn.02.00.dims");
            intermediaryEntity.AddExplicitMember("TaxObligationTypeDimension", "h05:PAYGI");
            var intermediaryEntityCtx = new Context { Entity = intermediaryEntity };
            instance.Contexts.Add(intermediaryEntityCtx);
            instance.SetMetricNamespace("pyid", "http://sbr.gov.au/icls/py/pyid/pyid.02.00.data");
            instance.AddFact(intermediaryEntityCtx.Entity.Segment, "Identifiers.TaxAgentNumber.Identifier", null, null, "123456789");
            output.WriteLine(instance.ToXml());
        }

        [Fact]
        public void InstanceWithNoScenarioMemberShouldNotHaveunusedNamespaces()
        {
            // set up a report that has no facts and therefore no contexts with scenarios containing any explicit or typed members
            // this means that the namespace "http://xbrl.org/2006/xbrldi" with the canonical prefix "xbrldi" is not used and should not be declared
            var inputfile = Path.Combine("data", "minimal.xbrl");
            var instance = Report.FromFile(inputfile);
            instance.Facts.RemoveAt(0);
            var xml = instance.ToXmlDocument();
            Assert.False(xml.DocumentElement.HasAttribute("xmlns:xbrldi"));
        }

        [Fact]
        public void DefaultNamespaceIsHandledCorrectly()
        {
            // should be completely the same instance
            // first has canonical prefix "xbrli" for "http://www.xbrl.org/2003/instance"
            // and second has it as default namespace 
            var first = Path.Combine("data", "reference.xbrl");
            var second = Path.Combine("data", "reference_defaultns.xbrl");
            var report = ReportComparer.Report(first, second);
            if (!report.Result)
                Console.WriteLine(report);
            Assert.True(report.Result);
        }
    }
}
