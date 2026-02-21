//
//  ReportComparerTests.cs
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
    using System.IO;
    using Diwen.Xbrl.Xml;
    using Diwen.Xbrl.Xml.Comparison;
    using Xunit;

    public static class ReportComparerTests
    {

        [Fact]
        public static void CompareReportToItself()
        {
            // load same instance twice and compare
            var path = Path.Combine("data", "xml", "reference.xbrl");
            var firstReport = Report.FromFile(path);
            var secondReport = Report.FromFile(path);
            var report = ReportComparer.Report(firstReport, secondReport);
            // comparison should find the instances equivalent
            Assert.True(report.Result);
            // there should be no differences reported
            Assert.Empty(report.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareReportToItselfWithPath()
        {
            // load same instance twice and compare
            var path = Path.Combine("data", "xml", "reference.xbrl");
            var report = ReportComparer.Report(path, path);
            // comparison should find the instances equivalent
            Assert.True(report.Result);
            // there should be no differences reported
            Assert.Empty(report.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void ComparisonReportContainsContextWithNullScenario()
        {
            // load same instance twice
            var path = Path.Combine("data", "xml", "reference.xbrl");
            var firstReport = Report.FromFile(path);
            var secondReport = Report.FromFile(path);

            // modify other one so they produce differences to report
            foreach (var context in secondReport.Contexts)
            {
                context.Entity.Identifier.Value = "00000000000000000098";
            }

            var comparisonReport = ReportComparer.Report(firstReport, secondReport);
            // comparison should find the instances different and not crash
            Assert.False(comparisonReport.Result);
            // there should be some differences reported
            Assert.NotEmpty(comparisonReport.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareBasicNullValues()
        {
            // load same instance twice and compare
            var path = Path.Combine("data", "xml", "reference.xbrl");
            var firstReport = Report.FromFile(path);
            var secondReport = Report.FromFile(path);

            secondReport.TaxonomyVersion = null;
            secondReport.SchemaReference = null;

            var comparisonReport = ReportComparer.Report(firstReport, secondReport, ComparisonTypes.Basic);
            // comparison should find the instances different and not throw
            Assert.False(comparisonReport.Result);
            Assert.NotEmpty(comparisonReport.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareSimilarFacts()
        {
            // load same instance twice and compare
            var path = Path.Combine("data", "xml", "reference.xbrl");
            var firstReport = Report.FromFile(path);
            var secondReport = Report.FromFile(path);

            secondReport.Facts[0].Value = "0";
            secondReport.Facts[1].Value = "0";

            var comparisonReport = ReportComparer.Report(firstReport, secondReport, ComparisonTypes.Facts);
            // comparison should find the instances different and not throw
            Assert.False(comparisonReport.Result);
            Assert.NotEmpty(comparisonReport.Messages);
            Assert.Equal(4, comparisonReport.Messages.Count); // report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareTotallyDifferentReports()
        {
            var firstPath = Path.Combine("data", "xml", "reference.xbrl");
            var secondPath = Path.Combine("data", "xml", "ars.xbrl");
            var comparisonReport = ReportComparer.Report(firstPath, secondPath, ComparisonTypes.All);
            Assert.False(comparisonReport.Result);
            Assert.NotEmpty(comparisonReport.Messages);
        }

        [Fact]
        public static void CompareDomainNamespacesOfTotallyDifferentReports()
        {
            var firstPath = Path.Combine("data", "xml", "reference.xbrl");
            var secondPath = Path.Combine("data", "xml", "ars.xbrl");
            var comparisonReport = ReportComparer.Report(firstPath, secondPath, ComparisonTypes.DomainNamespaces);
            Assert.False(comparisonReport.Result);
            Assert.NotEmpty(comparisonReport.Messages);
        }

        //[Fact]
        //public static void CompareInstancesTypedMemberDifferent()
        //{
        //	// load same instance twice
        //	var path = Path.Combine("data", "reference.xbrl");
        //	var firstInstance = Instance.FromFile(path);
        //	var secondInstance = Instance.FromFile(path);
        //	// change second only slightly and compare
        //	secondInstance.Contexts[1].Scenario.TypedMembers[0].Value = "abcd";
        //	var report = InstanceComparer.Report(firstInstance, secondInstance);
        //	// not the same anymore
        //	Assert.False(report.Result);
        //	// should contain some differences
        //	CollectionAssert.NotEmpty(report.Messages);
        //	// one context is different, report should reflect this once per instance
        //	Assert.Equal(2, report.Messages.Count, report.Messages.Join(Environment.NewLine));
        //}

        //[Fact]
        //public static void CompareLargeInstanceMinorDifferenceInContext()
        //{
        //	// load same instance twice
        //	var path = Path.Combine("data", "ars.xbrl");
        //	var firstInstance = Instance.FromFile(path);
        //	var secondInstance = Instance.FromFile(path);
        //	// change second only slightly and compare
        //	// original is s2c_VM:x5
        //	secondInstance.Contexts["CI22070"].Scenario.ExplicitMembers[5].Value = new XmlQualifiedName("s2c_VM:x6");
        //	var report = InstanceComparer.Report(firstInstance, secondInstance, ComparisonTypes.Contexts);
        //	// not the same anymore
        //	Assert.False(report.Result);
        //	// should contain some differences
        //	CollectionAssert.NotEmpty(report.Messages);
        //	// one context is different, report should reflect this once per instance
        //	Assert.Equal(2, report.Messages.Count, report.Messages.Join(Environment.NewLine));
        //}

        [Fact]
        public static void CompareLargeReportMinorDifferenceInFact()
        {
            // load same instance twice
            var path = Path.Combine("data", "xml", "ars.xbrl");
            var firstReport = Report.FromFile(path, removeUnusedObjects: false, collapseDuplicateContexts: false, removeDuplicateFacts: false);
            var secondReport = Report.FromFile(path, removeUnusedObjects: false, collapseDuplicateContexts: false, removeDuplicateFacts: false);
            // change one fact in both instances
            // original is 0
            firstReport.Facts[33099].Value = "FOOBAR";
            secondReport.Facts[33099].Value = "DEADBEEF";
            var comparisonReport = ReportComparer.Report(firstReport, secondReport, ComparisonTypes.Facts);
            // not the same anymore
            Assert.False(comparisonReport.Result);
            // should contain some differences
            Assert.NotEmpty(comparisonReport.Messages);
            // one fact is different, report should reflect this once per instance
            Assert.Equal(2, comparisonReport.Messages.Count);// report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareReportTest()
        {
            // load same instance twice
            var path = Path.Combine("data", "xml", "reference.xbrl");
            var report = ReportComparer.Report(path, path, ComparisonTypes.Contexts);
            Assert.True(report.Result, string.Join(Environment.NewLine, report.Messages));
        }

        [Fact]
        public static void CompareEntityWithNoEntity()
        {
            var first = Report.FromFile(Path.Combine("data", "xml", "empty_instance.xbrl"));
            var second = Report.FromFile(Path.Combine("data", "xml", "empty_instance.xbrl"));

            second.Entity = new Entity("LEI", "00000000000000000098");
            second.Period = new Period(2016, 05, 31);
            second.AddFilingIndicator("foo", false);
            // should not throw
            Assert.NotNull(ReportComparer.Report(first, second));
        }

        [Fact]
        public static void CompareDifferentEntityAndPeriodOnly()
        {
            var path = Path.Combine("data", "xml", "reference.xbrl");
            var path2 = Path.Combine("data", "xml", "reference2.xbrl");

            var report = ReportComparer.Report(path, path2);
            Assert.False(report.Result);
            string[] expectedMessages = {
                "Different Entity",
                "Different Period",
                "(a) Identifier=http://standards.iso.org/iso/17442:1234567890ABCDEFGHIJ",
                "(b) Identifier=http://standards.iso.org/iso/17442:00000000000000000098",
                "(a) Instant=2014-12-31",
                "(b) Instant=2015-12-31"
            };
            // Does NOT report the differences for each context
            Assert.Equal(expectedMessages, report.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareFactWithMissingUnit()
        {
            var path = Path.Combine("data", "xml", "reference.xbrl");
            var firstReport = Report.FromFile(path);
            var secondReport = Report.FromFile(path);

            // comparing this should not throw
            secondReport.Facts[0].Unit = null;

            var comparisonReport = ReportComparer.Report(firstReport, secondReport);
            Assert.False(comparisonReport.Result);
            Assert.Equal(2, comparisonReport.Messages.Count); //, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void BypassTaxonomyVersion()
        {
            var path = Path.Combine("data", "xml", "reference.xbrl");
            var firstReport = Report.FromFile(path);
            var secondReport = Report.FromFile(path);
            secondReport.TaxonomyVersion = null;

            var types = ComparisonTypes.All & ~ComparisonTypes.TaxonomyVersion;

            var comparisonReport = ReportComparer.Report(firstReport, secondReport, types);
            Assert.True(comparisonReport.Result);
        }

        [Fact]
        public static void ChooseBasicComparisons()
        {
            // Load same file twice
            // change the other a little, save it and reload so we get a fresh instance
            // bypassing some bugs that lead to not everything updating on the fly :(

            var path = Path.Combine("data", "xml", "minimal.xbrl");
            var tempPath = Path.GetTempFileName();

            var first = Report.FromFile(path);
            var second = Report.FromFile(path);

            var period = new Period(2050, 12, 31);
            var entity = new Entity("foo", "bar");

            foreach (var context in second.Contexts)
            {
                context.Period = period;
                context.Entity = entity;
            }

            second.ToFile(tempPath);
            second = Report.FromFile(tempPath);

            ComparisonReport report = null;
            string[] expectedMessages = null;
            BasicComparisons basicSelection;

            // Full comparison outputs both Entity and Period from basic and detailed comparion
            report = ReportComparer.Report(first, second, ComparisonTypes.All);
            Assert.False(report.Result);
            expectedMessages = new string[] {
                "Different Entity",
                "Different Period",
                $"(a) {first.Contexts[0].Entity}",
                $"(b) {second.Contexts[0].Entity}",
                $"(a) {first.Contexts[0].Period}",
                $"(b) {second.Contexts[0].Period}"
            };
            Assert.Equal(expectedMessages, report.Messages);//, report.Messages.Join(Environment.NewLine));

            // basic comparison with all flags reports Entity and Period
            basicSelection = BasicComparisons.All;
            report = ReportComparer.Report(first, second, ComparisonTypes.Basic);
            Assert.False(report.Result);
            expectedMessages = new string[] {
                "Different Entity",
                "Different Period",
            };
            Assert.Equal(expectedMessages, report.Messages);//, report.Messages.Join(Environment.NewLine));


            // basic comparison without Entity and Period flags does not list any differences
            basicSelection &= ~BasicComparisons.Entity;
            basicSelection &= ~BasicComparisons.Period;

            report = ReportComparer.Report(first, second, ComparisonTypes.Basic, basicSelection);
            Assert.True(report.Result);
        }

        [Fact]
        public static void CompareTotallyDifferentInstancesReportObjects()
        {
            var firstPath = Path.Combine("data", "xml", "reference.xbrl");
            var secondPath = Path.Combine("data", "xml", "ars.xbrl");
            var report = ReportComparer.ReportObjects(firstPath, secondPath);
            Assert.False(report.Result);
        }

        [Fact]
        public static void CompareLargeInstanceMinorDifferenceInFactReportObjects()
        {
            // load same instance twice
            var path = Path.Combine("data", "xml", "ars.xbrl");
            var firstReport = Report.FromFile(path, removeUnusedObjects: false, collapseDuplicateContexts: false, removeDuplicateFacts: false);
            var secondReport = Report.FromFile(path, removeUnusedObjects: false, collapseDuplicateContexts: false, removeDuplicateFacts: false);
            // change one fact in both instances
            // original is 0
            firstReport.Facts[33099].Value = "FOOBAR";
            secondReport.Facts[33099].Value = "DEADBEEF";
            var comparisonReport = ReportComparer.ReportObjects(firstReport, secondReport, ComparisonTypes.All, BasicComparisons.All);

            // not the same anymore
            Assert.False(comparisonReport.Result);

            // one fact is different, report should reflect this once per instance
            Assert.Single(comparisonReport.Facts.Item1);
            Assert.Single(comparisonReport.Facts.Item2);
        }

        [Fact]
        public static void ExamineFactDifferences()
        {
            // load same instance twice
            var path = Path.Combine("data", "xml", "ars.xbrl");
            var firstReport = Report.FromFile(path);
            var secondReport = Report.FromFile(path);

            // change some facts  on the second instance
            firstReport.Facts[0].Value = "DEADBEED";
            firstReport.Facts[1].Decimals = "16";

            secondReport.Facts[0].Value = "FOOBAR";
            secondReport.Facts[1].Decimals = "9";

            // change some context members
            secondReport.Contexts[4].AddExplicitMember("AO", "s2c_AO:x0");
            secondReport.Contexts[5].AddExplicitMember("RT", "s2c_RT:x52");

            // generate raw differences
            var comparisonReport = ReportComparer.ReportObjects(firstReport, secondReport, ComparisonTypes.All, BasicComparisons.All);
            // should give negative result because differences were found
            Assert.False(comparisonReport.Result);
            // should report both facts for both instances
            Assert.Equal(2, comparisonReport.Facts.Item1.Count);
            Assert.Equal(2, comparisonReport.Facts.Item2.Count);

            // try to match the facts
            var factReport = FactReport.FromReport(comparisonReport);
            Assert.NotNull(factReport.Matches);
        }

        [Fact]
        public static void CompareDifferentUnits()
        {
            var path = Path.Combine("data", "xml", "reference.xbrl");
            var first = Report.FromFile(path);
            var second = Report.FromFile(path);

            var unit = new Unit("uUSD", "iso4217:USD");
            second.Units.Add(unit);
            second.Facts[1].Unit = unit;
            second.Units.Remove("uEUR");

            // write+read just to make sure all references are updated internally :(
            var tmpfile = Path.GetTempFileName();
            second.ToFile(tmpfile);
            second = Report.FromFile(tmpfile);

            var report = ReportComparer.Report(first, second, ComparisonTypes.All);
            Assert.False(report.Result);
            var expectedMessages = new string[] {
                "Different Units",
                $"(a) {first.Facts[1]} ({first.Facts[1].Context.Scenario})",
                $"(b) {second.Facts[1]} ({second.Facts[1].Context.Scenario})",
                $"(a) {first.Facts[1].Unit}",
                $"(b) {second.Facts[1].Unit}",
            };
            Assert.Equal(expectedMessages, report.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void ReportsAreComparableAfterCreation()
        {
            var first = new Report();
            var second = new Report();

            var comparison = ReportComparer.Report(first, second);

            Assert.True(comparison.Result, string.Join(Environment.NewLine, comparison.Messages));
        }
    }
}
