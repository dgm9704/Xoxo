//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2024 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.Xbrl.Xml.Comparison
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Diwen.Xbrl.Extensions;

    /// <summary/>
    public static class ReportComparer
    {
        /// <summary/>
        public static ComparisonReport Report(string a, string b)
        => Report(Xml.Report.FromFile(a), Xml.Report.FromFile(b), ComparisonTypes.All);

        /// <summary/>
        public static ComparisonReport Report(string a, string b, ComparisonTypes comparisonTypes)
        => Report(Xml.Report.FromFile(a), Xml.Report.FromFile(b), comparisonTypes);

        /// <summary/>
        public static ComparisonReport Report(Stream a, Stream b)
        => Report(Xml.Report.FromStream(a), Xml.Report.FromStream(b), ComparisonTypes.All);

        /// <summary/>
        public static ComparisonReport Report(Stream a, Stream b, ComparisonTypes comparisonTypes)
        => Report(Xml.Report.FromStream(a), Xml.Report.FromStream(b), comparisonTypes);

        /// <summary/>
        public static ComparisonReport Report(Report a, Report b)
        => Report(a, b, ComparisonTypes.All);

        /// <summary/>
        public static ComparisonReport Report(Report a, Report b, ComparisonTypes comparisonTypes)
        => Report(a, b, comparisonTypes, BasicComparisons.All);

        /// <summary/>
        public static ComparisonReport Report(Report a, Report b, ComparisonTypes comparisonTypes, BasicComparisons basicComparisons)
        {
            var messages = new List<string>();

            if (comparisonTypes.HasFlag(ComparisonTypes.Basic))
                messages.AddRange(BasicComparison(a, b, basicComparisons));

            messages.AddRange(ComparisonMethods.
                            Where(c => comparisonTypes.HasFlag(c.Key)).
                              SelectMany(c => c.Value(a, b)));

            return new ComparisonReport(!messages.Any(), messages);
        }

        /// <summary/>
        public static ComparisonReportObjects ReportObjects(string a, string b)
        => ReportObjects(Xml.Report.FromFile(a), Xml.Report.FromFile(b), ComparisonTypes.All, BasicComparisons.All);

        /// <summary/>
        public static ComparisonReportObjects ReportObjects(Report a, Report b, ComparisonTypes comparisons, BasicComparisons basicSelection)
        {
            var report = new ComparisonReportObjects();

            if (comparisons.HasFlag(ComparisonTypes.Basic))
                report.Basics = BasicComparison(a, b, basicSelection).ToList();

            if (comparisons.HasFlag(ComparisonTypes.Contexts))
                report.Contexts = ScenarioComparison(a, b);

            if (comparisons.HasFlag(ComparisonTypes.Facts))
                report.Facts = FactComparison(a, b);

            if (comparisons.HasFlag(ComparisonTypes.DomainNamespaces))
                report.DomainNamespaces = DomainNamespaceComparison(a, b);

            if (comparisons.HasFlag(ComparisonTypes.Units))
                report.Units = UnitComparison(a, b);

            if (comparisons.HasFlag(ComparisonTypes.Entity))
                report.Entities = EntityComparison(a, b);

            if (comparisons.HasFlag(ComparisonTypes.Entity))
                report.Periods = PeriodComparison(a, b);

            if (comparisons.HasFlag(ComparisonTypes.TaxonomyVersion))
                report.TaxonomyVersions = TaxonomyVersionComparison(a, b);

            if (comparisons.HasFlag(ComparisonTypes.SchemaReference))
                report.SchemaReferences = SchemaReferenceComparison(a, b);

            if (comparisons.HasFlag(ComparisonTypes.FilingIndicators))
                report.FilingIndicators = FilingIndicatorComparison(a, b);

            report.Result = GetResultForReport(report);

            return report;
        }

        static bool GetResultForReport(ComparisonReportObjects report)
        => !(
                (report.Basics != null && report.Basics.Any())
                || (report.Contexts != null && (report.Contexts.Item1.Any() || report.Contexts.Item2.Any()))
                || (report.Facts != null && (report.Facts.Item1.Any()) || report.Facts.Item2.Any())
                || (report.DomainNamespaces != null && (report.DomainNamespaces.Item1.Any() || report.DomainNamespaces.Item2.Any()))
                || (report.Units != null && (report.Units.Item1.Any() || report.Units.Item2.Any()))
                || (report.Entities != null && (report.Entities.Item1.Any() || report.Entities.Item2.Any()))
                || (report.Periods != null && (report.Periods.Item1.Any() || report.Periods.Item2.Any()))
                || (report.TaxonomyVersions != null && (report.TaxonomyVersions.Item1.Any() || report.TaxonomyVersions.Item2.Any()))
                || (report.SchemaReferences != null && (report.SchemaReferences.Item1.Any() || report.SchemaReferences.Item2.Any()))
                || (report.FilingIndicators != null && (report.FilingIndicators.Item1.Any() || report.FilingIndicators.Item2.Any()))
                );

        static readonly Dictionary<ComparisonTypes, Func<Report, Report, IEnumerable<string>>> ComparisonMethods
            = new()
            {
                [ComparisonTypes.Contexts] = ScenarioComparisonMessages,
                [ComparisonTypes.Facts] = FactComparisonMessages,
                [ComparisonTypes.DomainNamespaces] = DomainNamespaceComparisonMessages,
                [ComparisonTypes.Units] = UnitComparisonMessages,
                [ComparisonTypes.Entity] = EntityComparisonMessages,
                [ComparisonTypes.Period] = PeriodComparisonMessages,
                [ComparisonTypes.TaxonomyVersion] = TaxonomyVersionComparisonMessages,
                [ComparisonTypes.SchemaReference] = SchemaReferenceComparisonMessages,
                [ComparisonTypes.FilingIndicators] = FilingIndicatorComparisonMessages
            };

        #region SimpleChecks

        static readonly Dictionary<BasicComparisons, Tuple<string, Func<Report, Report, bool>>> SimpleCheckMethods
        = new()
        {
            [BasicComparisons.NullReports] = Tuple.Create("At least one the reports is null", CheckNullReports),
            [BasicComparisons.SchemaReference] = Tuple.Create("Different SchemaReference", CheckSchemaReference),
            [BasicComparisons.Units] = Tuple.Create("Different Units", CheckUnits),
            [BasicComparisons.FilingIndicators] = Tuple.Create("Different FilingIndicators", CheckFilingIndicators),
            [BasicComparisons.ContextCount] = Tuple.Create("Different number of Contexts", CheckContextCount),
            [BasicComparisons.FactCount] = Tuple.Create("Different number of Facts", CheckFactCount),
            [BasicComparisons.DomainNamespaces] = Tuple.Create("Different domain namespaces", CheckDomainNamespaces),
            [BasicComparisons.Entity] = Tuple.Create("Different Entity", CheckEntity),
            [BasicComparisons.Period] = Tuple.Create("Different Period", CheckPeriod),
        };

        static IEnumerable<string> BasicComparison(Report a, Report b, BasicComparisons selection)
        => SimpleCheckMethods.
                Where(c => selection.HasFlag(c.Key)).
                Where(c => !c.Value.Item2(a, b)).
                Select(c => c.Value.Item1);

        static bool CheckNullReports(Report a, Report b)
        => a != null && b != null;

        static bool CheckTaxonomyVersion(Report a, Report b)
        => a.TaxonomyVersion != null && b.TaxonomyVersion != null
            ? a.TaxonomyVersion.Equals(b.TaxonomyVersion, StringComparison.Ordinal)
            : a.TaxonomyVersion == null && b.TaxonomyVersion == null;

        static bool CheckSchemaReference(Report a, Report b)
        => a.SchemaReference == null ? b.SchemaReference == null : a.SchemaReference.Equals(b.SchemaReference);

        static bool CheckUnits(Report a, Report b)
        => a.Units.Equals(b.Units);

        static bool CheckFilingIndicators(Report a, Report b)
        => a.FilingIndicators.Equals(b.FilingIndicators);

        static bool CheckCount<T>(ICollection<T> a, ICollection<T> b)
        => a != null && b != null
                ? a.Count == b.Count
                : a == null && b == null;

        static bool CheckContextCount(Report a, Report b)
        => CheckCount(a.Contexts, b.Contexts);

        static bool CheckFactCount(Report a, Report b)
        => CheckCount(a.Facts, b.Facts);

        static bool CheckDomainNamespaces(Report a, Report b)
        => a.GetUsedDomainNamespaces().
                ContentCompare(b.GetUsedDomainNamespaces());

        static bool CheckEntity(Report a, Report b)
        {
            Entity entityA = null;
            Entity entityB = null;
            if (a.Contexts != null && a.Contexts.Any())
            {
                entityA = a.Contexts.First().Entity;

                if (b.Contexts != null && b.Contexts.Any())
                    entityB = b.Contexts.First().Entity;
            }

            return (entityA == null && entityB == null)
            || (entityA != null && entityA.Equals(entityB));
        }

        static bool CheckPeriod(Report a, Report b)
        {
            Period periodA = null;
            Period periodB = null;
            if (a.Contexts != null && a.Contexts.Any())
            {
                periodA = a.Contexts.First().Period;

                if (b.Contexts != null && b.Contexts.Any())
                    periodB = b.Contexts.First().Period;
            }

            return (periodA == null && periodB == null)
            || (periodA != null && periodA.Equals(periodB));
        }

        #endregion

        #region DetailedChecks

        static Tuple<List<Context>, List<Context>> ContextComparison(Report a, Report b)
        => a.Contexts.ContentCompareReport(b.Contexts);

        static IEnumerable<string> ContextComparisonMessages(Report a, Report b)
        => ContextComparisonMessages(ContextComparison(a, b));

        static IEnumerable<string> ContextComparisonMessages(Tuple<List<Context>, List<Context>> differences)
        => differences.Item1.Select(item => $"(a) {item.Id}:" + (item.Scenario?.ToString() ?? "")).Concat(
            differences.Item2.Select(item => $"(b) {item.Id}:" + (item.Scenario?.ToString() ?? "")));

        static Tuple<List<Scenario>, List<Scenario>> ScenarioComparison(Report a, Report b)
        {
            var aList = a.
             Contexts.
             Select(c => c.Scenario).
             ToList();

            var bList = b.
                         Contexts.
                         Select(c => c.Scenario).
                         ToList();

            return aList.ContentCompareReport(bList);
        }

        static List<string> ScenarioComparisonMessages(Report a, Report b)
        {
            var differences = ScenarioComparison(a, b);
            var notInB = differences.Item1;
            var notInA = differences.Item2;

            var messages = new List<string>(notInB.Count + notInA.Count);

            if (notInB.Any())
            {
                // not until we're sure that there won't be duplicates
                // var aLookup = a.Contexts.ToDictionary(c => c.Scenario != null ? c.Scenario.ToString() : "", c => c.Id);
                var aLookup = GetScenarios(a);
                messages.AddRange(notInB.Select(item => ScenarioComparisonMessage(aLookup, item, "a")));
            }

            if (notInA.Any())
            {
                // not until we're sure that there won't be duplicates

                var bLookup = GetScenarios(b);
                messages.AddRange(notInA.Select(item => ScenarioComparisonMessage(bLookup, item, "b")));
            }

            return messages;
        }

        static string ScenarioComparisonMessage(Dictionary<string, string> contexts, Scenario item, string label)
        => $"({label}) {contexts[item?.ToString() ?? ""]}: {item}";

        static Dictionary<string, string> GetScenarios(Report instance)
        {
            var scenarios = new Dictionary<string, string>();
            foreach (var c in instance.Contexts)
                scenarios[c.Scenario?.ToString() ?? ""] = c.Id;

            return scenarios;
        }

        static Tuple<List<Fact>, List<Fact>> FactComparison(Report a, Report b)
        => a.Facts.ContentCompareReport(b.Facts);

        static IEnumerable<string> FactComparisonMessages(Report a, Report b)
        => FactComparisonMessages(FactComparison(a, b));

        static IEnumerable<string> FactComparisonMessages(Tuple<List<Fact>, List<Fact>> differences)
        => differences.Item1.Select(f => FactComparisonMessage(f, "a")).
            Concat(differences.Item2.Select(f => FactComparisonMessage(f, "b")));

        static string FactComparisonMessage(Fact fact, string label)
        => $"({label}) {fact} ({fact.Context.Scenario})";

        static Tuple<List<string>, List<string>> DomainNamespaceComparison(Report a, Report b)
        => a.GetUsedDomainNamespaces().
                ContentCompareReport(b.GetUsedDomainNamespaces());

        static IEnumerable<string> DomainNamespaceComparisonMessages(Report a, Report b)
        => ComparisonMessages(DomainNamespaceComparison(a, b));

        static Tuple<List<Unit>, List<Unit>> UnitComparison(Report a, Report b)
        => a.Units.ContentCompareReport(b.Units);

        static IEnumerable<string> UnitComparisonMessages(Report a, Report b)
        => ComparisonMessages(a.Units.ContentCompareReport(b.Units));

        static Tuple<List<Identifier>, List<Identifier>> EntityComparison(Report a, Report b)
        {
            var aList = new List<Identifier>();
            var bList = new List<Identifier>();

            if (a.Contexts != null && a.Contexts.Any())
                aList.Add(a.Contexts.First().Entity.Identifier);

            if (b.Contexts != null && b.Contexts.Any())
                bList.Add(b.Contexts.First().Entity.Identifier);

            return aList.ContentCompareReport(bList);
        }

        static IEnumerable<string> EntityComparisonMessages(Report a, Report b)
        => EntityComparisonMessages(EntityComparison(a, b));

        static IEnumerable<string> EntityComparisonMessages(Tuple<List<Identifier>, List<Identifier>> differences)
        => differences.Item1.Select(item => $"(a) Identifier={item}").Concat(
            differences.Item2.Select(item => $"(b) Identifier={item}"));

        static Tuple<List<Period>, List<Period>> PeriodComparison(Report a, Report b)
        {
            var aList = new List<Period>();
            var bList = new List<Period>();

            if (a.Contexts != null && a.Contexts.Any())
                aList.Add(a.Contexts.First().Period);

            if (b.Contexts != null && b.Contexts.Any())
                bList.Add(b.Contexts.First().Period);

            return aList.ContentCompareReport(bList);
        }

        static IEnumerable<string> PeriodComparisonMessages(Report a, Report b)
        => ComparisonMessages(PeriodComparison(a, b));

        static Tuple<List<string>, List<string>> TaxonomyVersionComparison(Report a, Report b)
        {
            var aList = new List<string>();
            var bList = new List<string>();

            aList.Add(a.TaxonomyVersion);
            bList.Add(b.TaxonomyVersion);

            return aList.ContentCompareReport(bList);
        }

        static IEnumerable<string> TaxonomyVersionComparisonMessages(Report a, Report b)
        => TaxonomyVersionComparisonMessages(TaxonomyVersionComparison(a, b));

        static IEnumerable<string> TaxonomyVersionComparisonMessages(Tuple<List<string>, List<string>> differences)
        => differences.Item1.Select(item => $"(a) taxonomy-version: {item}").Concat(
            differences.Item2.Select(item => $"(b) taxonomy-version: {item}"));

        static Tuple<List<SchemaReference>, List<SchemaReference>> SchemaReferenceComparison(Report a, Report b)
        {
            var aList = new List<SchemaReference>();
            var bList = new List<SchemaReference>();

            aList.Add(a.SchemaReference);
            bList.Add(b.SchemaReference);

            return aList.ContentCompareReport(bList);
        }

        static IEnumerable<string> SchemaReferenceComparisonMessages(Report a, Report b)
        => ComparisonMessages(SchemaReferenceComparison(a, b));

        static Tuple<List<FilingIndicator>, List<FilingIndicator>> FilingIndicatorComparison(Report a, Report b)
        => a.FilingIndicators.Where(fi => fi.Filed).ToList().
            ContentCompareReport(b.FilingIndicators.Where(fi => fi.Filed).ToList());

        static IEnumerable<string> FilingIndicatorComparisonMessages(Report a, Report b)
        => ComparisonMessages(FilingIndicatorComparison(a, b));

        static IEnumerable<string> ComparisonMessages<T>(Tuple<List<T>, List<T>> differences)
        => differences.Item1.Select(item => $"(a) {item}").Concat(
        differences.Item2.Select(item => $"(b) {item}"));

        #endregion
    }
}