//
//  InstanceComparerTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2018 John Nordberg
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
    using System.IO;
    using Xunit;

    public static class InstanceComparerTests
    {

        [Fact]
        public static void CompareInstanceToItself()
        {
            // load same instance twice and compare
            var path = Path.Combine("data", "reference.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);
            var report = InstanceComparer.Report(firstInstance, secondInstance);
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            // comparison should find the instances equivalent
            Assert.True(report.Result);
            // there should be no differences reported
            Assert.Empty(report.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareInstanceToItselfWithPath()
        {
            // load same instance twice and compare
            var path = Path.Combine("data", "reference.xbrl");
            var report = InstanceComparer.Report(path, path);
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            // comparison should find the instances equivalent
            Assert.True(report.Result);
            // there should be no differences reported
            Assert.Empty(report.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void ComparisonReportContainsContextWithNullScenario()
        {
            // load same instance twice
            var path = Path.Combine("data", "reference.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);

            // modify other one so they produce differences to report 
            foreach (var context in secondInstance.Contexts)
            {
                context.Entity.Identifier.Value = "00000000000000000098";
            }

            var report = InstanceComparer.Report(firstInstance, secondInstance);
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            // comparison should find the instances different and not crash
            Assert.False(report.Result);
            // there should be some differences reported
            Assert.NotEmpty(report.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareBasicNullValues()
        {
            // load same instance twice and compare
            var path = Path.Combine("data", "reference.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);

            secondInstance.TaxonomyVersion = null;
            secondInstance.SchemaReference = null;

            var report = InstanceComparer.Report(firstInstance, secondInstance, ComparisonTypes.Basic);
            // comparison should find the instances different and not throw
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            Assert.False(report.Result);
            Assert.NotEmpty(report.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareSimilarFacts()
        {
            // load same instance twice and compare
            var path = Path.Combine("data", "reference.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);

            secondInstance.Facts[0].Value = "0";
            secondInstance.Facts[1].Value = "0";

            var report = InstanceComparer.Report(firstInstance, secondInstance, ComparisonTypes.Facts);
            // comparison should find the instances different and not throw
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            Assert.False(report.Result);
            Assert.NotEmpty(report.Messages);
            Assert.Equal(4, report.Messages.Count); // report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareTotallyDifferentInstances()
        {
            var firstPath = Path.Combine("data", "reference.xbrl");
            var secondPath = Path.Combine("data", "ars.xbrl");
            var report = InstanceComparer.Report(firstPath, secondPath, ComparisonTypes.All);
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            Assert.False(report.Result);
            Assert.NotEmpty(report.Messages);
        }

        [Fact]
        public static void CompareDomainNamespacesOfTotallyDifferentInstances()
        {
            var firstPath = Path.Combine("data", "reference.xbrl");
            var secondPath = Path.Combine("data", "ars.xbrl");
            var report = InstanceComparer.Report(firstPath, secondPath, ComparisonTypes.DomainNamespaces);
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            Assert.False(report.Result);
            Assert.NotEmpty(report.Messages);
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
        //	//Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
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
        //	//Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
        //	// not the same anymore
        //	Assert.False(report.Result);
        //	// should contain some differences
        //	CollectionAssert.NotEmpty(report.Messages);
        //	// one context is different, report should reflect this once per instance
        //	Assert.Equal(2, report.Messages.Count, report.Messages.Join(Environment.NewLine));
        //}

        [Fact]
        public static void CompareLargeInstanceMinorDifferenceInFact()
        {
            // load same instance twice
            var path = Path.Combine("data", "ars.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);
            // change one fact in both instances
            // original is 0
            firstInstance.Facts[33099].Value = "FOOBAR";
            secondInstance.Facts[33099].Value = "DEADBEEF";
            var report = InstanceComparer.Report(firstInstance, secondInstance, ComparisonTypes.Facts);
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            // not the same anymore
            Assert.False(report.Result);
            // should contain some differences
            Assert.NotEmpty(report.Messages);
            // one fact is different, report should reflect this once per instance
            Assert.Equal(2, report.Messages.Count);// report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareReportTest()
        {
            // load same instance twice
            var path = Path.Combine("data", "reference.xbrl");
            var report = InstanceComparer.Report(path, path, ComparisonTypes.Contexts);
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            Assert.True(report.Result, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void CompareEntityWithNoEntity()
        {
            var first = Instance.FromFile(Path.Combine("data", "empty_instance.xbrl"));
            var second = Instance.FromFile(Path.Combine("data", "empty_instance.xbrl"));

            second.Entity = new Entity("LEI", "00000000000000000098");
            second.Period = new Period(2016, 05, 31);
            second.AddFilingIndicator("foo", false);
            // should not throw 
            Assert.NotNull(InstanceComparer.Report(first, second));
        }

        [Fact]
        public static void CompareDifferentEntityAndPeriodOnly()
        {
            var path = Path.Combine("data", "reference.xbrl");
            var path2 = Path.Combine("data", "reference2.xbrl");

            var report = InstanceComparer.Report(path, path2);
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
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
            var path = Path.Combine("data", "reference.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);

            // comparing this should not throw
            secondInstance.Facts[0].Unit = null;

            var report = InstanceComparer.Report(firstInstance, secondInstance);
            //Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            Assert.False(report.Result);
            Assert.Equal(2, report.Messages.Count); //, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void BypassTaxonomyVersion()
        {
            var path = Path.Combine("data", "reference.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);
            secondInstance.TaxonomyVersion = null;

            var types = ComparisonTypes.All & ~ComparisonTypes.TaxonomyVersion;

            var report = InstanceComparer.Report(firstInstance, secondInstance, types);
            Assert.True(report.Result);
        }

        [Fact]
        public static void ChooseBasicComparisons()
        {
            // Load same file twice
            // change the other a little, save it and reload so we get a fresh instance
            // bypassing some bugs that lead to not everything updating on the fly :(

            var path = Path.Combine("data", "minimal.xbrl");
            var tempPath = Path.GetTempFileName();

            var first = Instance.FromFile(path);
            var second = Instance.FromFile(path);

            var period = new Period(2050, 12, 31);
            var entity = new Entity("foo", "bar");

            foreach (var context in second.Contexts)
            {
                context.Period = period;
                context.Entity = entity;
            }

            second.ToFile(tempPath);
            second = Instance.FromFile(tempPath);

            ComparisonReport report = null;
            string[] expectedMessages = null;
            BasicComparisons basicSelection;

            // Full comparison outputs both Entity and Period from basic and detailed comparion
            report = InstanceComparer.Report(first, second, ComparisonTypes.All);
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
            report = InstanceComparer.Report(first, second, ComparisonTypes.Basic);
            Assert.False(report.Result);
            expectedMessages = new string[] {
                "Different Entity",
                "Different Period",
            };
            Assert.Equal(expectedMessages, report.Messages);//, report.Messages.Join(Environment.NewLine));


            // basic comparison without Entity and Period flags does not list any differences
            basicSelection &= ~BasicComparisons.Entity;
            basicSelection &= ~BasicComparisons.Period;

            report = InstanceComparer.Report(first, second, ComparisonTypes.Basic, basicSelection);
            Assert.True(report.Result);
        }

        [Fact]
        public static void CompareTotallyDifferentInstancesReportObjects()
        {
            var firstPath = Path.Combine("data", "reference.xbrl");
            var secondPath = Path.Combine("data", "ars.xbrl");
            var report = InstanceComparer.ReportObjects(firstPath, secondPath);
            Assert.False(report.Result);
        }

        [Fact]
        public static void CompareLargeInstanceMinorDifferenceInFactReportObjects()
        {
            // load same instance twice
            var path = Path.Combine("data", "ars.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);
            // change one fact in both instances
            // original is 0
            firstInstance.Facts[33099].Value = "FOOBAR";
            secondInstance.Facts[33099].Value = "DEADBEEF";
            var report = InstanceComparer.ReportObjects(firstInstance, secondInstance, ComparisonTypes.All, BasicComparisons.All);

            // not the same anymore
            Assert.False(report.Result);

            // one fact is different, report should reflect this once per instance
            Assert.Single(report.Facts.Item1);
            Assert.Single(report.Facts.Item2);
        }

        [Fact]
        public static void ExamineFactDifferences()
        {
            // load same instance twice
            var path = Path.Combine("data", "ars.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);

            // change some facts  on the second instance
            firstInstance.Facts[0].Value = "DEADBEED";
            firstInstance.Facts[1].Decimals = "16";

            secondInstance.Facts[0].Value = "FOOBAR";
            secondInstance.Facts[1].Decimals = "9";

            // change some context members
            secondInstance.Contexts[4].AddExplicitMember("AO", "s2c_AO:x0");
            secondInstance.Contexts[5].AddExplicitMember("RT", "s2c_RT:x52");

            // generate raw differences
            var report = InstanceComparer.ReportObjects(firstInstance, secondInstance, ComparisonTypes.All, BasicComparisons.All);
            // should give negative result because differences were found
            Assert.False(report.Result);
            // should report both facts for both instances
            Assert.Equal(2, report.Facts.Item1.Count);
            Assert.Equal(2, report.Facts.Item2.Count);

            // try to match the facts
            var factReport = FactReport.FromReport(report);
            Assert.NotNull(factReport.Matches);
        }

        [Fact]
        public static void CompareDifferentUnits()
        {
            var path = Path.Combine("data", "reference.xbrl");
            var first = Instance.FromFile(path);
            var second = Instance.FromFile(path);

            var unit = new Unit("uUSD", "iso4217:USD");
            second.Units.Add(unit);
            second.Facts[1].Unit = unit;
            second.Units.Remove("uEUR");

            // write+read just to make sure all references are updated internally :(
            var tmpfile = Path.GetTempFileName();
            second.ToFile(tmpfile);
            second = Instance.FromFile(tmpfile);

            var report = InstanceComparer.Report(first, second, ComparisonTypes.All);
            Assert.False(report.Result);
            var expectedMessages = new string[] {
                "Different Units",
                $"(a) {first.Facts[1]} ({first.Facts[1].Context.Scenario})",
                $"(b) {second.Facts[1]} ({second.Facts[1].Context.Scenario})",
                "(a) iso4217:EUR",
                "(b) iso4217:USD",
            };
            Assert.Equal(expectedMessages, report.Messages);//, report.Messages.Join(Environment.NewLine));
        }

        [Fact]
        public static void InstancesAreComparableAfterCreation()
        {
            var first = new Instance();
            var second = new Instance();

            var comparison = InstanceComparer.Report(first, second);

            Assert.True(comparison.Result, comparison.Messages.Join(Environment.NewLine));
        }

    }
}