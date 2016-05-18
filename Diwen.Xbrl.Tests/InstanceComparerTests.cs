//
//  InstanceComparerTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2016 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.Xbrl.Tests
{
    using System;
    using System.IO;
    using System.Xml;
    using NUnit.Framework;

    [TestFixture]
    public static class InstanceComparerTests
    {
        [Test]
        public static void CompareInstanceToItself()
        {
            // load same instance twice and compare
            var path = Path.Combine("data", "reference.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);
            var report = InstanceComparer.Report(firstInstance, secondInstance, ComparisonTypes.All);
            Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            // comparison should find the instances equivalent
            Assert.IsTrue(report.Result);
            // there should be no differences reported
            CollectionAssert.IsEmpty(report.Messages);
        }

        [Test]
        public static void ComparisonReportContainsContextWithNullScenario()
        {
            // load same instance twice
            var path = Path.Combine("data", "reference.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);

            // modify other one so they produce differences to report 
            foreach(var context in secondInstance.Contexts)
            {
                context.Entity.Identifier.Value = "00000000000000000098";
            }

            var report = InstanceComparer.Report(firstInstance, secondInstance, ComparisonTypes.All);
            Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            // comparison should find the instances different and not crash
            Assert.IsFalse(report.Result);
            // there should be some differences reported
            CollectionAssert.IsNotEmpty(report.Messages);
        }

        [Test]
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
            Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            Assert.IsFalse(report.Result);
            CollectionAssert.IsNotEmpty(report.Messages);
        }

        [Test]
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
            Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            Assert.IsFalse(report.Result);
            CollectionAssert.IsNotEmpty(report.Messages);
            Assert.AreEqual(4, report.Messages.Count);
        }

        [Test]
        public static void CompareTotallyDifferentInstances()
        {
            var firstPath = Path.Combine("data", "reference.xbrl");
            var secondPath = Path.Combine("data", "ars.xbrl");
            var firstInstance = Instance.FromFile(firstPath);
            var secondInstance = Instance.FromFile(secondPath);
            var report = InstanceComparer.Report(firstInstance, secondInstance, ComparisonTypes.Basic);
            Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            Assert.IsFalse(report.Result);
            CollectionAssert.IsNotEmpty(report.Messages);
        }

        [Test]
        public static void CompareInstancesTypedMemberDifferent()
        {
            // load same instance twice
            var path = Path.Combine("data", "reference.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);
            // change second only slightly and compare
            secondInstance.Contexts[1].Scenario.TypedMembers[0].Value = "abcd";
            var report = InstanceComparer.Report(firstInstance, secondInstance);
            Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            // not the same anymore
            Assert.IsFalse(report.Result);
            // should contain some differences
            CollectionAssert.IsNotEmpty(report.Messages);
            // one context is different, report should reflect this once per instance
            Assert.AreEqual(2, report.Messages.Count);
        }

        [Test]
        public static void CompareLargeInstanceMinorDifferenceInContext()
        {
            // load same instance twice
            var path = Path.Combine("data", "ars.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);
            // change second only slightly and compare
            // original is s2c_VM:x5
            secondInstance.Contexts["CI22070"].Scenario.ExplicitMembers[5].Value = new XmlQualifiedName("s2c_VM:x6");
            var report = InstanceComparer.Report(firstInstance, secondInstance, ComparisonTypes.Contexts);
            Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            // not the same anymore
            Assert.IsFalse(report.Result);
            // should contain some differences
            CollectionAssert.IsNotEmpty(report.Messages);
            // one context is different, report should reflect this once per instance
            Assert.AreEqual(2, report.Messages.Count);
        }

        [Test]
        public static void CompareLargeInstanceMinorDifferenceInFact()
        {
            // load same instance twice
            var path = Path.Combine("data", "ars.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);
            // change second only slightly and compare
            // original is 0
            secondInstance.Facts[33099].Value = "0.0";
            var report = InstanceComparer.Report(firstInstance, secondInstance, ComparisonTypes.Facts);
            Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            // not the same anymore
            Assert.IsFalse(report.Result);
            // should contain some differences
            CollectionAssert.IsNotEmpty(report.Messages);
            // one context is different, report should reflect this once per instance
            Assert.AreEqual(2, report.Messages.Count);
        }

        [Test]
        public static void CompareReportTest()
        {
            // load same instance twice
            var path = Path.Combine("data", "reference.xbrl");
            var firstInstance = Instance.FromFile(path);
            var secondInstance = Instance.FromFile(path);
            var report = InstanceComparer.Report(firstInstance, secondInstance, ComparisonTypes.Contexts);
            Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
            Assert.IsTrue(report.Result);
        }

        [Test]
        public static void CompareEntityWithNoEntity()
        {
            var first = Instance.FromFile(Path.Combine("data", "empty_instance.xbrl"));
            var second = Instance.FromFile(Path.Combine("data", "empty_instance.xbrl"));

            second.Entity = new Entity("LEI", "00000000000000000098");
            second.Period = new Period(2016, 05, 31);
            second.AddFilingIndicator("foo", false);
            // should not throw 
            Assert.IsNotNull(InstanceComparer.Report(first, second, ComparisonTypes.All));
        }
    }
}

