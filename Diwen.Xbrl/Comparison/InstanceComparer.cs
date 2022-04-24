//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2020 John Nordberg
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

namespace Diwen.Xbrl.Comparison
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Diwen.Xbrl.Extensions;

	public static class InstanceComparer
	{
		public static ComparisonReport Report(string a, string b)
		=> Report(Instance.FromFile(a), Instance.FromFile(b), ComparisonTypes.All);

		public static ComparisonReport Report(string a, string b, ComparisonTypes comparisonTypes)
		=> Report(Instance.FromFile(a), Instance.FromFile(b), comparisonTypes);

		public static ComparisonReport Report(Stream a, Stream b)
		=> Report(Instance.FromStream(a), Instance.FromStream(b), ComparisonTypes.All);

		public static ComparisonReport Report(Stream a, Stream b, ComparisonTypes comparisonTypes)
		=> Report(Instance.FromStream(a), Instance.FromStream(b), comparisonTypes);

		public static ComparisonReport Report(Instance a, Instance b)
		=> Report(a, b, ComparisonTypes.All);

		public static ComparisonReport Report(Instance a, Instance b, ComparisonTypes comparisonTypes)
		=> Report(a, b, comparisonTypes, BasicComparisons.All);

		public static ComparisonReport Report(Instance a, Instance b, ComparisonTypes comparisonTypes, BasicComparisons basicComparisons)
		{
			var messages = new List<string>();

			if (comparisonTypes.HasFlag(ComparisonTypes.Basic))
				messages.AddRange(BasicComparison(a, b, basicComparisons));

			messages.AddRange(ComparisonMethods.
							Where(c => comparisonTypes.HasFlag(c.Key)).
							  SelectMany(c => c.Value(a, b)));

			return new ComparisonReport(!messages.Any(), messages);
		}

		public static ComparisonReportObjects ReportObjects(string a, string b)
		=> ReportObjects(Instance.FromFile(a), Instance.FromFile(b), ComparisonTypes.All, BasicComparisons.All);

		public static ComparisonReportObjects ReportObjects(Instance a, Instance b, ComparisonTypes comparisons, BasicComparisons basicSelection)
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

		static Dictionary<ComparisonTypes, Func<Instance, Instance, IEnumerable<string>>> ComparisonMethods
			= new Dictionary<ComparisonTypes, Func<Instance, Instance, IEnumerable<string>>>
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

		static Dictionary<BasicComparisons, Tuple<string, Func<Instance, Instance, bool>>> SimpleCheckMethods
		= new Dictionary<BasicComparisons, Tuple<string, Func<Instance, Instance, bool>>>
		{
			[BasicComparisons.NullInstances] = Tuple.Create<string, Func<Instance, Instance, bool>>("At least one the instances is null", CheckNullInstances),
			[BasicComparisons.SchemaReference] = Tuple.Create<string, Func<Instance, Instance, bool>>("Different SchemaReference", CheckSchemaReference),
			[BasicComparisons.Units] = Tuple.Create<string, Func<Instance, Instance, bool>>("Different Units", CheckUnits),
			[BasicComparisons.FilingIndicators] = Tuple.Create<string, Func<Instance, Instance, bool>>("Different FilingIndicators", CheckFilingIndicators),
			[BasicComparisons.ContextCount] = Tuple.Create<string, Func<Instance, Instance, bool>>("Different number of Contexts", CheckContextCount),
			[BasicComparisons.FactCount] = Tuple.Create<string, Func<Instance, Instance, bool>>("Different number of Facts", CheckFactCount),
			[BasicComparisons.DomainNamespaces] = Tuple.Create<string, Func<Instance, Instance, bool>>("Different domain namespaces", CheckDomainNamespaces),
			[BasicComparisons.Entity] = Tuple.Create<string, Func<Instance, Instance, bool>>("Different Entity", CheckEntity),
			[BasicComparisons.Period] = Tuple.Create<string, Func<Instance, Instance, bool>>("Different Period", CheckPeriod)
		};

		static IEnumerable<string> BasicComparison(Instance a, Instance b, BasicComparisons selection)
		=> SimpleCheckMethods.
				Where(c => selection.HasFlag(c.Key)).
				Where(c => !c.Value.Item2(a, b)).
				Select(c => c.Value.Item1);

		static bool CheckNullInstances(object a, object b)
		=> (a != null && b != null);

		static bool CheckTaxonomyVersion(Instance a, Instance b)
		=> a.TaxonomyVersion != null && b.TaxonomyVersion != null
			? a.TaxonomyVersion.Equals(b.TaxonomyVersion, StringComparison.Ordinal)
			: a.TaxonomyVersion == null && b.TaxonomyVersion == null;

		static bool CheckSchemaReference(Instance a, Instance b)
		=> a.SchemaReference == null ? b.SchemaReference == null : a.SchemaReference.Equals(b.SchemaReference);

		static bool CheckUnits(Instance a, Instance b)
		=> a.Units.Equals(b.Units);

		static bool CheckFilingIndicators(Instance a, Instance b)
		=> a.FilingIndicators.Equals(b.FilingIndicators);

		static bool CheckCount<T>(ICollection<T> a, ICollection<T> b)
		=> a != null && b != null
				? a.Count == b.Count
				: a == null && b == null;

		static bool CheckContextCount(Instance a, Instance b)
		=> CheckCount(a.Contexts, b.Contexts);

		static bool CheckFactCount(Instance a, Instance b)
		=> CheckCount(a.Facts, b.Facts);

		static bool CheckDomainNamespaces(Instance a, Instance b)
		=> a.GetUsedDomainNamespaces().
				ContentCompare(b.GetUsedDomainNamespaces());

		static bool CheckEntity(Instance a, Instance b)
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

		static bool CheckPeriod(Instance a, Instance b)
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

		static Tuple<List<Context>, List<Context>> ContextComparison(Instance a, Instance b)
		=> a.Contexts.ContentCompareReport(b.Contexts);

		static IEnumerable<string> ContextComparisonMessages(Instance a, Instance b)
		=> ContextComparisonMessages(ContextComparison(a, b));

		static IEnumerable<string> ContextComparisonMessages(Tuple<List<Context>, List<Context>> differences)
		=> differences.Item1.Select(item => $"(a) {item.Id}:" + (item.Scenario?.ToString() ?? "")).Concat(
			differences.Item2.Select(item => $"(b) {item.Id}:" + (item.Scenario?.ToString() ?? "")));

		static Tuple<List<Scenario>, List<Scenario>> ScenarioComparison(Instance a, Instance b)
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


		static IEnumerable<string> ScenarioComparisonMessages(Instance a, Instance b)
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

		static Dictionary<string, string> GetScenarios(Instance instance)
		{
			var scenarios = new Dictionary<string, string>();
			foreach (var c in instance.Contexts)
				scenarios[c.Scenario?.ToString() ?? ""] = c.Id;

			return scenarios;
		}

		static Tuple<List<Fact>, List<Fact>> FactComparison(Instance a, Instance b)
		=> a.Facts.ContentCompareReport(b.Facts);

		static IEnumerable<string> FactComparisonMessages(Instance a, Instance b)
		=> FactComparisonMesssages(FactComparison(a, b));

		static IEnumerable<string> FactComparisonMesssages(Tuple<List<Fact>, List<Fact>> differences)
		=> differences.Item1.Select(f => FactComparisonMessage(f, "a")).
			Concat(differences.Item2.Select(f => FactComparisonMessage(f, "b")));

		static string FactComparisonMessage(Fact fact, string label)
		=> $"({label}) {fact} ({fact.Context.Scenario})";

		static Tuple<List<string>, List<string>> DomainNamespaceComparison(Instance a, Instance b)
		=> a.GetUsedDomainNamespaces().
				ContentCompareReport(b.GetUsedDomainNamespaces());

		static IEnumerable<string> DomainNamespaceComparisonMessages(Instance a, Instance b)
		=> ComparisonMessages(DomainNamespaceComparison(a, b));

		static Tuple<List<Unit>, List<Unit>> UnitComparison(Instance a, Instance b)
		=> a.Units.ContentCompareReport(b.Units);

		static IEnumerable<string> UnitComparisonMessages(Instance a, Instance b)
		=> ComparisonMessages(a.Units.ContentCompareReport(b.Units));

		static Tuple<List<Identifier>, List<Identifier>> EntityComparison(Instance a, Instance b)
		{
			var aList = new List<Identifier>();
			var bList = new List<Identifier>();

			if (a.Contexts != null && a.Contexts.Any())
				aList.Add(a.Contexts.First().Entity.Identifier);

			if (b.Contexts != null && b.Contexts.Any())
				bList.Add(b.Contexts.First().Entity.Identifier);

			return aList.ContentCompareReport(bList);
		}

		static IEnumerable<string> EntityComparisonMessages(Instance a, Instance b)
		=> EntityComparisonMessages(EntityComparison(a, b));

		static IEnumerable<string> EntityComparisonMessages(Tuple<List<Identifier>, List<Identifier>> differences)
		=> differences.Item1.Select(item => $"(a) Identifier={item}").Concat(
			differences.Item2.Select(item => $"(b) Identifier={item}"));

		static Tuple<List<Period>, List<Period>> PeriodComparison(Instance a, Instance b)
		{
			var aList = new List<Period>();
			var bList = new List<Period>();

			if (a.Contexts != null && a.Contexts.Any())
				aList.Add(a.Contexts.First().Period);

			if (b.Contexts != null && b.Contexts.Any())
				bList.Add(b.Contexts.First().Period);

			return aList.ContentCompareReport(bList);
		}

		static IEnumerable<string> PeriodComparisonMessages(Instance a, Instance b)
		=> ComparisonMessages(PeriodComparison(a, b));

		static Tuple<List<string>, List<string>> TaxonomyVersionComparison(Instance a, Instance b)
		{
			var aList = new List<string>();
			var bList = new List<string>();

			aList.Add(a.TaxonomyVersion);
			bList.Add(b.TaxonomyVersion);

			return aList.ContentCompareReport(bList);
		}

		static IEnumerable<string> TaxonomyVersionComparisonMessages(Instance a, Instance b)
		=> TaxonomyVersionComparisonMessages(TaxonomyVersionComparison(a, b));

		static IEnumerable<string> TaxonomyVersionComparisonMessages(Tuple<List<string>, List<string>> differences)
		=> differences.Item1.Select(item => $"(a) taxonomy-version: {item}").Concat(
			differences.Item2.Select(item => $"(b) taxonomy-version: {item}"));

		static Tuple<List<SchemaReference>, List<SchemaReference>> SchemaReferenceComparison(Instance a, Instance b)
		{
			var aList = new List<SchemaReference>();
			var bList = new List<SchemaReference>();

			aList.Add(a.SchemaReference);
			bList.Add(b.SchemaReference);

			return aList.ContentCompareReport(bList);
		}

		static IEnumerable<string> SchemaReferenceComparisonMessages(Instance a, Instance b)
		=> ComparisonMessages(SchemaReferenceComparison(a, b));

		static Tuple<List<FilingIndicator>, List<FilingIndicator>> FilingIndicatorComparison(Instance a, Instance b)
		=> a.FilingIndicators.Where(fi => fi.Filed).ToList().
			ContentCompareReport(b.FilingIndicators.Where(fi => fi.Filed).ToList());

		static IEnumerable<string> FilingIndicatorComparisonMessages(Instance a, Instance b)
		=> ComparisonMessages(FilingIndicatorComparison(a, b));

		static IEnumerable<string> ComparisonMessages<T>(Tuple<List<T>, List<T>> differences)
		=> differences.Item1.Select(item => $"(a) {item}").Concat(
		differences.Item2.Select(item => $"(b) {item}"));

		#endregion
	}
}

