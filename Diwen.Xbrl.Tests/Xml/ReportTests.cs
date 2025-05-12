//
//  ReportTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2024 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Xunit;
    using Xunit.Abstractions;
    using Diwen.Xbrl.Xml.Comparison;
    using Diwen.Xbrl.Xml;
    using System.Xml.Linq;
    using System.Linq;

    public class ReportTests
    {

        private readonly ITestOutputHelper output;

        public ReportTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        internal Report CreateSolvencyReport()
        {
            // Sets default namespaces and units PURE, EUR
            var report = new Report();

            // When an explicit member is added, check that the namespace for the domain has been set
            report.CheckExplicitMemberDomainExists = true;

            // Initialize to the correct framework, module, taxonomy
            // The content is NOT validated against taxonomy or module schema
            // set module
            report.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");

            // set taxonomy
            report.TaxonomyVersion = "1.5.2.c";

            // "basic" namespaces
            // These are used for adding correct prefixes for different elements
            report.SetMetricNamespace("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met");
            report.SetDimensionNamespace("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
            report.SetTypedDomainNamespace("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");

            // Namespaces for actual reported values that belong to a domain (explicit members)
            report.AddDomainNamespace("s2c_CS", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/CS");
            report.AddDomainNamespace("s2c_AM", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/AM");

            // Add reporter and period
            // These will be reused across all contexts by default
            // Scheme or value are NOT validated
            report.Entity = new Entity("http://standards.iso.org/iso/17442", "1234567890ABCDEFGHIJ");
            report.Period = new Period(2014, 12, 31);

            // Any units that aren't used will be excluded during serialization
            // So it's safe to add extra units if needed
            report.Units.Add("uEUR", "iso4217:EUR");
            report.Units.Add("uPURE", "xbrli:pure");
            report.Units.Add("uFOO", "foo:bar");

            // Add filing indicators
            // These are NOT validated against actual reported values
            report.AddFilingIndicator("S.01.01");
            report.AddFilingIndicator("S.02.02");

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
            report.AddFact(scenario, "pi545", "uPURE", "4", "0.2547");

            // if a scenario with the given values already exists in the instance, it will be reused
            // you don't have to check for duplicates
            report.AddFact(scenario, "mi363", "uEUR", "-3", "45345");

            // Non - existing unit throws KeyNotFoundException
            try
            {
                report.AddFact(scenario, "mi363", "uSEK", "-3", "45345");
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
                report.AddFact(invalidScenario, "mi252", "uEUR", "4", "123456");
            }
            catch (InvalidOperationException ex)
            {
                output.WriteLine(ex.Message);
            }

            return report;
        }

        [Fact]
        public void WriteSolvencyReport()
        {
            var report = CreateSolvencyReport();
            report.AddDomainNamespace("s2c_XX", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/XX");
            // Write the instace to a file
            string path = "output.xbrl.xml";
            report.ToFile(path);
        }

        [Fact]
        public void ReadSolvencyReferenceReport()
        {
            var path = Path.Combine("data", "reference.xbrl");
            var referenceReport = Report.FromFile(path);
            Assert.NotNull(referenceReport);
        }

        [Fact]
        public void CompareSolvencyReferenceReport()
        {
            var report = CreateSolvencyReport();

            // unless done when loading, duplicate objects 
            // aren't automatically removed until serialization so do it before comparisons
            report.RemoveUnusedObjects();

            var referencePath = Path.Combine("data", "reference.xbrl");
            var referenceReport = Report.FromFile(referencePath);

            // Instances are functionally equivalent:
            // They have the same number of contexts and scenarios of the contexts match member-by-member
            // Members are checked by dimension, domain and value, namespaces included
            // They have the same facts matched by metric, value, decimals and unit
            // Entity and Period are also matched
            // Some things are NOT checked, eg. taxonomy version, context names
            //Assert.Equal(instance, referenceInstance);

            string tempFile = "temp.xbrl";
            report.ToFile(tempFile);

            var newReport = Report.FromFile(tempFile);

            // Assert.True(newInstance.Equals(instance));
            var comparison = ReportComparer.ReportObjects(report, newReport, ComparisonTypes.All, BasicComparisons.All);
            if (!comparison.Result)
                Console.WriteLine(report);
            Assert.True(comparison.Result);

            Assert.True(newReport.Equals(referenceReport));

            newReport.Contexts[1].AddExplicitMember("AM", "s2c_AM:x1");

            Assert.False(newReport.Equals(referenceReport));
        }

        [Fact]
        public void RoundtripCompareExampleReportArs()
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
            Report report = null;
            using (var stream = new FileStream(inputPath, FileMode.Open))
                report = Report.FromStream(stream, removeUnusedObjects: false, collapseDuplicateContexts: false, removeDuplicateFacts: false);

            Assert.Equal(2, report.Contexts.Count);

            report.CollapseDuplicateContexts();
            Assert.Single(report.Contexts);

            report = Report.FromFile(inputPath);
            Assert.Single(report.Contexts);
        }

        [Fact]
        public void ReadExampleReportFPInd()
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

            var comparison = ReportComparer.Report(first, second);
            Assert.True(comparison.Result, string.Join(Environment.NewLine, comparison.Messages));
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
            var report = CreateSolvencyReport();
            report.CheckExplicitMemberDomainExists = true;

            var scenario = new Scenario();
            scenario.AddTypedMember("CE", "ID", null);
            report.AddFact(scenario, "mi1234", null, null, "123");
            report.RemoveUnusedObjects();
            report.ToFile("typedmembernil.xbrl.xml");
        }

        [Fact]
        public void WriteToXmlDocument()
        => CreateSolvencyReport().
                                   ToXmlDocument().
                                   Save("xbrl2doc.xml");

        [Fact]
        public void NoMembers()
        {
            var report = new Report();
            report.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");
            report.TaxonomyVersion = "1.5.2.c";
            report.SetMetricNamespace("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met");
            report.SetDimensionNamespace("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
            report.SetTypedDomainNamespace("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");
            report.Entity = new Entity("http://standards.iso.org/iso/17442", "00000000000000000098");
            report.Period = new Period(2015, 12, 31);

            Scenario nullScenario = null;
            report.AddFact(nullScenario, "foo0", null, null, "alice");

            //Context nullContext = null;
            //instance.AddFact(nullContext, "foo1", null, null, "bob"); // Sorry bob

            var contextWithNullScenario = new Context();
            report.AddFact(contextWithNullScenario, "foo2", null, null, "carol");

            var contextWithNoMembers = new Context();
            contextWithNoMembers.Scenario = new Scenario();
            report.AddFact(contextWithNoMembers, "foo3", null, null, "dave");

            var scenarioWithNoMembers = new Scenario();
            report.AddFact(scenarioWithNoMembers, "foo4", null, null, "erin");

            Assert.Single(report.Contexts);
            Assert.Equal(4, report.Facts.Count);
        }

        [Fact]
        public void FiledOrNot()
        {
            var report = new Report();
            report.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");
            report.TaxonomyVersion = "1.5.2.c";
            report.SetMetricNamespace("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met");
            report.SetDimensionNamespace("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
            report.SetTypedDomainNamespace("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");
            report.Entity = new Entity("http://standards.iso.org/iso/17442", "00000000000000000098");
            report.Period = new Period(2015, 12, 31);

            report.AddFilingIndicator("foo", true);
            report.AddFilingIndicator("bar", false);
        }

        [Fact]
        public void EnumeratedFactValueNamespace()
        {
            // Create a minimal test instance
            var report = new Report();
            report.SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd");
            report.TaxonomyVersion = "1.5.2.c";
            report.SetMetricNamespace("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met");
            report.Entity = new Entity("http://standards.iso.org/iso/17442", "00000000000000000098");
            report.Period = new Period(2015, 12, 31);

            // add a fact with enumerated value 
            report.AddFact((Scenario)null, "ei1643", null, null, "s2c_CN:x1");

            // add the namespace for the domain
            report.AddDomainNamespace("s2c_CN", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/CN");

            // write the instance to file and read it back
            string file = "EnumeratedFactValueNamespace.xbrl";
            report.ToFile(file);
            report = Report.FromFile(file);

            // instance should still contain the namespace for the domain
            Assert.Equal("http://eiopa.europa.eu/xbrl/s2c/dict/dom/CN", report.Namespaces.LookupNamespace("s2c_CN"));
        }

        [Fact]
        public void ReadAndWriteComments()
        {
            // read a test instance with a comment
            var inputPath = Path.Combine("data", "comments.xbrl");
            var report = Report.FromFile(inputPath);
            Assert.Contains("foo", report.Comments);

            // add a new comment
            report.Comments.Add("bar");
            var outputPath = Path.Combine("data", "morecomments.xbrl");
            report.ToFile(outputPath);
            report = Report.FromFile(outputPath);
            Assert.Contains("bar", report.Comments);
        }

        [Fact]
        public void NoExtraNamespaces()
        {
            var report = Report.FromFile(Path.Combine("data", "comments.xbrl"));
            report.SetDimensionNamespace("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
            report.SetTypedDomainNamespace("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");
            report.ToFile("ns.out");
        }

        [Fact]
        public void EmptyInstance()
        {
            // should load ok
            var report = Report.FromFile(Path.Combine("data", "empty_instance.xbrl"));
            Assert.NotNull(report);
            report.ToFile("empty_instance_out.xbrl");
        }

        [Fact]
        public void ReportFromString()
        {
            var input = File.ReadAllText(Path.Combine("data", "comments.xbrl"));
            var report = Report.FromXml(input);
            var output = report.ToXml();
            Assert.NotEmpty(output);
            // Most probably wont't match due to differences in casing or apostrophe vs. quotation etc.
            //Assert.Equal(input, output);
        }

        [Fact]
        public void SerializedReportWithNoMonetaryUnitShouldNotHaveUnusedNamespace()
        {
            var inFile = Path.Combine("data", "minimal.xbrl");
            var report = Report.FromFile(inFile);
            var outFile = "minimal.out";
            report.ToFile(outFile);
            var filecontent = File.ReadAllText(outFile);
            Assert.DoesNotContain("iso4217", filecontent);
        }

        [Fact]
        public void ExplicitMembersWithSurroundingWhitespaceShouldNotBork()
        {
            var infile = Path.Combine("data", "example_erst_dcca.xbrl");
            var report = Report.FromFile(infile);
            Assert.NotNull(report);
        }

        [Fact]
        public void FactWithNullContextShouldNotThrow_71()
        {
            var infile = Path.Combine("data", "71.xbrl");
            var report = Report.FromFile(infile);
            Assert.NotNull(report);
        }

        [Fact]
        public void ExplicitMemberNamespaceOverwriting_72()
        {
            var report = new Report();
            var intermediaryEntity = new Entity("http://www.ato.gov.au/abn", "123456789") { Report = report };
            report.SetDimensionNamespace("h04", "http://sbr.gov.au/dims/RprtPyType.02.00.dims");
            intermediaryEntity.AddExplicitMember("ReportPartyTypeDimension", "h04:Intermediary");
            report.SetDimensionNamespace("h05", "http://sbr.gov.au/dims/TaxOblgtn.02.00.dims");
            intermediaryEntity.AddExplicitMember("TaxObligationTypeDimension", "h05:PAYGI");
            var intermediaryEntityCtx = new Context { Entity = intermediaryEntity };
            report.Contexts.Add(intermediaryEntityCtx);
            report.SetMetricNamespace("pyid", "http://sbr.gov.au/icls/py/pyid/pyid.02.00.data");
            report.AddFact(intermediaryEntityCtx.Entity.Segment, "Identifiers.TaxAgentNumber.Identifier", null, null, "123456789");
            output.WriteLine(report.ToXml());
        }

        [Fact]
        public void ReportWithNoScenarioMemberShouldNotHaveunusedNamespaces()
        {
            // set up a report that has no facts and therefore no contexts with scenarios containing any explicit or typed members
            // this means that the namespace "http://xbrl.org/2006/xbrldi" with the canonical prefix "xbrldi" is not used and should not be declared
            var inputfile = Path.Combine("data", "minimal.xbrl");
            var report = Report.FromFile(inputfile);
            report.Facts.RemoveAt(0);
            var xml = report.ToXmlDocument();
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

        [Fact]
        public void RemoveDeclarationAndProcessingInstructionsAndComments()
        {
            var inputpath = Path.Combine("data", "minimal.xbrl");

            var doc = XDocument.Load(inputpath);
            Assert.NotNull(doc.Declaration);
            var processingInstructions = doc.Nodes().OfType<XProcessingInstruction>();
            Assert.Contains(processingInstructions, pi => pi.Target == "instance-generator");
            Assert.Contains(processingInstructions, pi => pi.Target == "taxonomy-version");
            var comments = doc.Nodes().OfType<XComment>();
            Assert.Contains(comments, c => c.Value == "foo");

            var report = Report.FromFile(inputpath);
            report.OutputInstanceGenerator = false;
            report.OutputTaxonomyVersion = false;
            report.OutputXmlDeclaration = false;
            report.OutputComments = false;

            var outputpath = "output_nopi.xbrl";
            report.ToFile(outputpath);
            doc = XDocument.Load(outputpath);

            Assert.Null(doc.Declaration);
            processingInstructions = doc.Nodes().OfType<XProcessingInstruction>();
            Assert.Empty(processingInstructions);

            comments = doc.Nodes().OfType<XComment>();
            Assert.Empty(comments);

        }
    }
}
