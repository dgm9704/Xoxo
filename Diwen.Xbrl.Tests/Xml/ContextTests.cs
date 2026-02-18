//
//  ContextsTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2026 John Nordberg
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
    using System.Diagnostics;
    using Diwen.Xbrl.Xml;
    using Diwen.Xbrl.Xml.Comparison;
    using Xunit;
    using Xunit.Abstractions;

    public class ContextTests
    {

        // 			<xbrli:context id="end2014">
        //     <xbrli:entity>
        //       <xbrli:identifier scheme="http://www.xbrl.fi">1234567-1</xbrli:identifier>
        //     </xbrli:entity>
        //     <xbrli:period>
        //       <xbrli:instant>2014-12-31</xbrli:instant>
        //     </xbrli:period>
        //   </xbrli:context>

        private readonly ITestOutputHelper output;

        public ContextTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void CompareContextsWithSameEntity()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var left = new Context
            {
                Entity = entity
            };
            var right = new Context
            {
                Entity = entity
            };

            Assert.Equal(left, right);
        }

        [Fact]
        public void CompareContextsWithDifferentEntity()
        {
            var leftentity = new Entity("http://www.xbrl.fi", "1234567-1");
            var rightentity = new Entity("http://www.xbrl.fi", "0123456-7");
            var left = new Context
            {
                Entity = leftentity
            };
            var right = new Context
            {
                Entity = rightentity
            };

            Assert.NotEqual(left, right);
        }

        [Fact]
        public void CompareContextsWithSameInstant()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var period = new Period(2014, 12, 31);

            var left = new Context
            {
                Entity = entity,
                Period = period
            };

            var right = new Context
            {
                Entity = entity,
                Period = period
            };

            Assert.Equal(left, right);
        }

        [Fact]
        public static void CompareContextsWithDifferentInstant()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var leftperiod = new Period(2014, 12, 31);
            var rightperiod = new Period(2014, 11, 30);

            var left = new Context
            {
                Entity = entity,
                Period = leftperiod
            };

            var right = new Context
            {
                Entity = entity,
                Period = rightperiod
            };

            Assert.NotEqual(left, right);
        }

        [Fact]
        public void CompareContextsWithSameSpan()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var period = new Period(2014, 12, 01, 2014, 12, 31);

            var left = new Context
            {
                Entity = entity,
                Period = period
            };

            var right = new Context
            {
                Entity = entity,
                Period = period
            };

            Assert.Equal(left, right);
        }

        [Fact]
        public void CompareContextsWithDifferentSpan()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var leftperiod = new Period(2014, 12, 01, 2014, 12, 31);
            var rightperiod = new Period(2014, 11, 01, 2014, 11, 30);

            var left = new Context
            {
                Entity = entity,
                Period = leftperiod
            };

            var right = new Context
            {
                Entity = entity,
                Period = rightperiod
            };

            Assert.NotEqual(left, right);
        }

        [Fact]
        public void CompareContextsWithDifferentTypeOfPeriod()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var leftperiod = new Period(2014, 12, 31);
            var rightperiod = new Period(2014, 12, 1, 2014, 12, 31);

            var left = new Context
            {
                Entity = entity,
                Period = leftperiod
            };

            var right = new Context
            {
                Entity = entity,
                Period = rightperiod
            };

            Assert.NotEqual(left, right);
        }

        //   <xbrli:context id="Context_Instant_IntangibleRights">
        //     <xbrli:entity>
        //       <xbrli:identifier scheme="http://www.xbrl.fi">1234567-1</xbrli:identifier>
        //       <xbrli:segment>
        //         <xbrldi:explicitMember dimension="fi-sbr-dim-D033:IntangibleAssetsBreakdownDimension">fi-sbr-dim-D033:IntangibleRights</xbrldi:explicitMember>
        //       </xbrli:segment>
        //     </xbrli:entity>
        //     <xbrli:period>
        //       <xbrli:instant>2013-12-31</xbrli:instant>
        //     </xbrli:period>
        //   </xbrli:context>

        [Fact]
        public void CompareContextsWithSameSegment()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var period = new Period(2013, 12, 31);

            var segment = new Segment();
            segment.AddExplicitMember("dimPrefix:dimCode", "valuePrefix:valueCode");
            entity.Segment = segment;

            var left = new Context
            {
                Entity = entity,
                Period = period
            };



            var right = new Context
            {
                Entity = entity,
                Period = period
            };

            Assert.Equal(left, right);
        }

        [Fact]
        public void CompareContextsWithDifferentSegment()
        {
            var period = new Period(2013, 12, 31);

            var leftentity = new Entity("http://www.xbrl.fi", "1234567-1");
            var leftsegment = new Segment();
            leftsegment.AddExplicitMember("dimPrefix:dimCode", "valuePrefix:valueCode");
            leftentity.Segment = leftsegment;

            var left = new Context
            {
                Entity = leftentity,
                Period = period
            };


            var rightentity = new Entity("http://www.xbrl.fi", "1234567-1");
            var rightsegment = new Segment();
            rightsegment.AddExplicitMember("dimPrefix:dimOtherCode", "valuePrefix:valueOtherCode");
            rightentity.Segment = rightsegment;

            var right = new Context
            {
                Entity = rightentity,
                Period = period
            };

            Assert.NotEqual(left, right);
        }

        [Fact]
        public void AddLargeNumberOfContexts()
        {
            // This test demonstrates the usual scenario of adding contexts to an instance
            // There is a performance hit that comes from checking each new context
            // against existing ones to avoid duplicates (GetContext)
            // This of course does not scale well

            // Duplicate checking and removing can be done at caller side when
            // creating the scenarios and/or after adding all contexts
            // So now there is a new method (CreateContext)
            // that does not check for duplicates

            var numberOfRuns = 5000;
            Report report;
            Scenario scenario;

            Stopwatch sw;

            report = CreateTestReport();
            sw = Stopwatch.StartNew();
            Context context;
            for (int i = 0; i < numberOfRuns; i++)
            {
                scenario = new Scenario();
                scenario.ExplicitMembers.Add("BL", "s2c_LB:x142");
                scenario.ExplicitMembers.Add("CS", "s2c_CS:x26");
                scenario.ExplicitMembers.Add("LX", "s2c_GA:LU");
                scenario.ExplicitMembers.Add("PI", "s2c_PI:x1");
                scenario.ExplicitMembers.Add("VG", "s2c_AM:x80");
                scenario.TypedMembers.Add("XX", "ID", "12345");
                scenario.ExplicitMembers.Add("VL", $"s2c_VM:x{i}"); // <- change one member slightly so each context is different
                scenario.Report = report;

                // GetContext compares the scenario to existing ones
                // and returns the match if found
                // creates and returns a new one if not found
                context = report.GetContext(scenario);

                report.AddFact(context, "mi363", "uEUR", "-3", $"{i}");
            }

            //output.WriteLine($"instance made with GetContext has {instance.Contexts.Count} contexts");

            // removing duplicates should not be needed but
            // make sure we do the same cleanup for both methods
            // so any timing is comparable
            report.CollapseDuplicateContexts();
            report.RemoveUnusedObjects();
            sw.Stop();
            output.WriteLine($"GetContext {sw.Elapsed}");
            report.ToFile("GetContext.xbrl");

            // load a minimal instance to work with
            // and setup a scenario to add
            report = CreateTestReport();
            sw = Stopwatch.StartNew();
            for (int i = 0; i < numberOfRuns; i++)
            {
                scenario = new Scenario();
                scenario.ExplicitMembers.Add("BL", "s2c_LB:x142");
                scenario.ExplicitMembers.Add("CS", "s2c_CS:x26");
                scenario.ExplicitMembers.Add("LX", "s2c_GA:LU");
                scenario.ExplicitMembers.Add("PI", "s2c_PI:x1");
                scenario.ExplicitMembers.Add("VG", "s2c_AM:x80");
                scenario.TypedMembers.Add("XX", "ID", "12345");
                scenario.ExplicitMembers.Add("VL", $"s2c_VM:x{i}"); // <- change one member slightly so each context is different
                scenario.Report = report;
                // CreateContext always creates and returns a new context
                // without overhead of checking existing ones for duplicates
                context = report.CreateContext(scenario);

                report.AddFact(context, "mi363", "uEUR", "-3", $"{i}");
            }

            //output.WriteLine($"instance made with CreateContext has {instance.Contexts.Count} contexts");

            // since we didn't check for existing matching scenarios
            // there can be duplicates that need to be cleaned up
            report.CollapseDuplicateContexts();
            report.RemoveUnusedObjects();
            sw.Stop();
            output.WriteLine($"CreateContext {sw.Elapsed}");
            report.ToFile("CreateContext.xbrl");

            // both methods should produce same result
            var result = ReportComparer.Report("GetContext.xbrl", "CreateContext.xbrl");
            Assert.True(result.Result, string.Join(Environment.NewLine, result.Messages));

        }

        private static Report CreateTestReport()
        {
            var report = new Report
            {
                SchemaReference = new SchemaReference("simple", "http://eiopa.europa.eu/eu/xbrl/s2md/fws/solvency/solvency2/2014-12-23/mod/ars.xsd"),
                TaxonomyVersion = "1.2.3",
                Entity = new Entity("http://standards.iso.org/iso/17442", "1234567890ABCDEFGHIJ"),
                Period = new Period(2014, 12, 31)
            };

            report.Units.Add("uEUR", "iso4217:EUR");
            report.Units.Add("uPURE", "xbrli:pure");

            report.SetMetricNamespace("s2md_met", "http://eiopa.europa.eu/xbrl/s2md/dict/met");
            report.SetTypedDomainNamespace("s2c_typ", "http://eiopa.europa.eu/xbrl/s2c/dict/typ");
            report.SetDimensionNamespace("s2c_dim", "http://eiopa.europa.eu/xbrl/s2c/dict/dim");
            report.AddDomainNamespace("s2c_LB", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/LB");
            report.AddDomainNamespace("s2c_CS", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/CS");
            report.AddDomainNamespace("s2c_GA", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/GA");
            report.AddDomainNamespace("s2c_PI", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/PI");
            report.AddDomainNamespace("s2c_AM", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/AM");
            report.AddDomainNamespace("s2c_VM", "http://eiopa.europa.eu/xbrl/s2c/dict/dom/VM");
            return report;
        }
    }
}
