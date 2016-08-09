//
//  This file is part of Diwen.Xbrl.
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

namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public static class InstanceComparer
    {
        static IFormatProvider ic = CultureInfo.InvariantCulture;

        public static ComparisonReport Report(string a, string b)
        {
            return Report(Instance.FromFile(a), Instance.FromFile(b), ComparisonTypes.All);
        }

        public static ComparisonReport Report(string a, string b, ComparisonTypes comparisonTypes)
        {
            return Report(Instance.FromFile(a), Instance.FromFile(b), comparisonTypes);
        }

        public static ComparisonReport Report(Stream a, Stream b)
        {
            return Report(Instance.FromStream(a), Instance.FromStream(b), ComparisonTypes.All);
        }

        public static ComparisonReport Report(Stream a, Stream b, ComparisonTypes comparisonTypes)
        {
            return Report(Instance.FromStream(a), Instance.FromStream(b), comparisonTypes);
        }

        public static ComparisonReport Report(Instance a, Instance b)
        {
            return Report(a, b, ComparisonTypes.All);
        }

        public static ComparisonReport Report(Instance a, Instance b, ComparisonTypes comparisonTypes)
        {
            var messages = ComparisonMethods.
                            Where(c => comparisonTypes.HasFlag(c.Key)).
                            SelectMany(c => c.Value(a, b)).
                            ToList();
            return new ComparisonReport(!messages.Any(), messages);
        }

        static Dictionary<ComparisonTypes, Func<Instance, Instance, IEnumerable<string>>> ComparisonMethods
        = new Dictionary<ComparisonTypes, Func<Instance, Instance, IEnumerable<string>>> {
            { ComparisonTypes.Basic, BasicComparison },
            { ComparisonTypes.Contexts, ScenarioComparison },
            { ComparisonTypes.Facts, FactComparison },
            { ComparisonTypes.DomainNamespaces, DomainNamespaceComparison },
            { ComparisonTypes.Units, UnitComparison },
            { ComparisonTypes.Entity, EntityComparison },
            { ComparisonTypes.Period, PeriodComparison },
            { ComparisonTypes.TaxonomyVersion, TaxonomyVersionComparison },
            { ComparisonTypes.SchemaReference, SchemaReferenceComparison },
            { ComparisonTypes.FilingIndicators, FilingIndicatorComparison },
        };

        #region SimpleChecks

        static Dictionary<string, Func<Instance, Instance, bool>> SimpleCheckMethods 
        = new Dictionary<string, Func<Instance, Instance, bool>> {
            { "At least one the instances is null", CheckNullInstances },
            { "Different TaxonomyVersion", CheckTaxonomyVersion },
            { "Different SchemaReference", CheckSchemaReference },
            { "Different Units", CheckUnits },
            { "Different FilingIndicators", CheckFilingIndicators },
            { "Different number of Contexts", CheckContextCount },
            { "Different number of Facts", CheckFactCount },
            { "Different domain namespaces", CheckDomainNamespaces },
            { "Different Entity", CheckEntity },
            { "Different Period", CheckPeriod },
        };

        static IEnumerable<string> BasicComparison(Instance a, Instance b)
        {
            return SimpleCheckMethods.
                Where(check => !check.Value(a, b)).
                Select(check => check.Key);
        }

        static bool CheckNullInstances(object a, object b)
        {
            return (a != null && b != null);
        }

        static bool CheckTaxonomyVersion(Instance a, Instance b)
        { 
            return a.TaxonomyVersion != null && b.TaxonomyVersion != null
                ? a.TaxonomyVersion.Equals(b.TaxonomyVersion, StringComparison.Ordinal)
                : a.TaxonomyVersion == null && b.TaxonomyVersion == null;
        }

        static bool CheckSchemaReference(Instance a, Instance b)
        {
            return a.SchemaReference.Equals(b.SchemaReference);
        }

        static bool CheckUnits(Instance a, Instance b)
        {
            return a.Units.Equals(b.Units);
        }

        static bool CheckFilingIndicators(Instance a, Instance b)
        {
            return a.FilingIndicators.Equals(b.FilingIndicators);
        }

        static bool CheckCount<T>(ICollection<T> a, ICollection<T> b)
        {
            return a != null && b != null 
                ? a.Count == b.Count 
                : a == null && b == null;
        }

        static bool CheckContextCount(Instance a, Instance b)
        {
            return CheckCount(a.Contexts, b.Contexts);
        }

        static bool CheckFactCount(Instance a, Instance b)
        {
            return CheckCount(a.Facts, b.Facts);
        }

        static bool CheckDomainNamespaces(Instance a, Instance b)
        {
            return a.GetUsedDomainNamespaces().
                ContentCompare(b.GetUsedDomainNamespaces());
        }

        static bool CheckEntity(Instance a, Instance b)
        {
            Entity entityA = null;
            Entity entityB = null;
            if(a.Contexts != null && a.Contexts.Count != 0)
            {
                entityA = a.Contexts.First().Entity;

                if(b.Contexts != null && b.Contexts.Count != 0)
                {
                    entityB = b.Contexts.First().Entity;
                }
            }

            return (entityA == null && entityB == null)
            || (entityA != null && entityA.Equals(entityB));
        }

        static bool CheckPeriod(Instance a, Instance b)
        {
            Period periodA = null;
            Period periodB = null;
            if(a.Contexts != null && a.Contexts.Count != 0)
            {
                periodA = a.Contexts.First().Period;

                if(b.Contexts != null && b.Contexts.Count != 0)
                {
                    periodB = b.Contexts.First().Period;
                }
            }

            return (periodA == null && periodB == null)
            || (periodA != null && periodA.Equals(periodB));
        }

        #endregion

        #region DetailedChecks

        static IEnumerable<string> ContextComparison(Instance a, Instance b)
        {
            var differences = a.Contexts.ContentCompareReport(b.Contexts);
            var messages = new List<string>(differences.Item1.Count + differences.Item2.Count);
            messages.AddRange(differences.Item1.Select(item => "(a) " + item.Id + ":" + (item.Scenario != null ? item.Scenario.ToString() : String.Empty)));
            messages.AddRange(differences.Item2.Select(item => "(b) " + item.Id + ":" + (item.Scenario != null ? item.Scenario.ToString() : String.Empty)));
            return messages;
        }

        static IEnumerable<string> ScenarioComparison(Instance a, Instance b)
        {
            var aList = new List<Scenario>(a.Contexts.Count);

            foreach(var c in a.Contexts)
            {
                aList.Add(c.Scenario);
            }

            var bList = new List<Scenario>(b.Contexts.Count);
            foreach(var c in b.Contexts)
            {
                bList.Add(c.Scenario);
            }

            var differences = aList.ContentCompareReport(bList);

            var notInB = differences.Item1;
            var notInA = differences.Item2;

            var messages = new List<string>(notInB.Count + notInA.Count);

            if(notInB.Any())
            {
                // not until we're sure that there won't be duplicates
                // var aLookup = a.Contexts.ToDictionary(c => c.Scenario != null ? c.Scenario.ToString() : "", c => c.Id);
                var aLookup = new Dictionary<string, string>();
                foreach(var c in a.Contexts)
                {
                    string key = c.Scenario != null ? c.Scenario.ToString() : "";
                    aLookup[key] = c.Id;
                }

                foreach(var item in notInB)
                {
                    var key = item != null ? item.ToString() : "";
                    var contextId = aLookup[key];
                    messages.Add(string.Format("(a) {0}: {1}", contextId, item));
                }
            }

            if(notInA.Any())
            {
                // not until we're sure that there won't be duplicates
                // var bLookup = b.Contexts.ToDictionary(c => c.Scenario != null ? c.Scenario.ToString() : "", c => c.Id);
                var bLookup = new Dictionary<string, string>();
                foreach(var c in b.Contexts)
                {
                    string key = c.Scenario != null ? c.Scenario.ToString() : "";
                    bLookup[key] = c.Id;
                }

                foreach(var item in notInA)
                {
                    var key = item != null ? item.ToString() : "";
                    var contextId = bLookup[key];
                    messages.Add(string.Format("(b) {0}: {1}", contextId, item));
                }
            }
            return messages;
        }

        static IEnumerable<string> FactComparison(Instance a, Instance b)
        {
            var differences = a.Facts.ContentCompareReport(b.Facts);
            var result = new List<string>(differences.Item1.Count + differences.Item2.Count);
            result.AddRange(differences.Item1.Select(item => string.Format(ic, "(a) {0} ({1})", item, item.Context.Scenario)));
            result.AddRange(differences.Item2.Select(item => string.Format(ic, "(b) {0} ({1})", item, item.Context.Scenario)));
            return result;
        }

        static IEnumerable<string> DomainNamespaceComparison(Instance a, Instance b)
        {
            var differences = a.GetUsedDomainNamespaces().
                ContentCompareReport(b.GetUsedDomainNamespaces());

            var result = new List<string>(differences.Item1.Count + differences.Item2.Count);
            result.AddRange(differences.Item1.Select(item => "(a) " + item));
            result.AddRange(differences.Item2.Select(item => "(b) " + item));
            return result;
        }

        static IEnumerable<string> UnitComparison(Instance a, Instance b)
        {
            var differences = a.Units.
                ContentCompareReport(b.Units);

            return differences.Item1.Select(item => "(a) " + item).
                Concat(differences.Item2.Select(item => "(b) " + item)).
                OrderBy(m => m);
        }

        static IEnumerable<string> EntityComparison(Instance a, Instance b)
        {
            var aList = new List<Entity>();
            var bList = new List<Entity>();

            if(a.Contexts != null && a.Contexts.Count != 0)
            {
                aList.Add(a.Contexts.First().Entity);
            }

            if(b.Contexts != null && b.Contexts.Count != 0)
            {
                bList.Add(b.Contexts.First().Entity);
            }

            var differences = aList.ContentCompareReport(bList);

            return differences.Item1.Select(item => "(a) " + item).
            Concat(differences.Item2.Select(item => "(b) " + item));   

        }

        static IEnumerable<string> PeriodComparison(Instance a, Instance b)
        {
            var aList = new List<Period>();
            var bList = new List<Period>();

            if(a.Contexts != null && a.Contexts.Count != 0)
            {
                aList.Add(a.Contexts.First().Period);
            }

            if(b.Contexts != null && b.Contexts.Count != 0)
            {
                bList.Add(b.Contexts.First().Period);
            }

            var differences = aList.ContentCompareReport(bList);

            return differences.Item1.Select(item => "(a) " + item).
                Concat(differences.Item2.Select(item => "(b) " + item));   
        }

        static IEnumerable<string> TaxonomyVersionComparison(Instance a, Instance b)
        {
            var aList = new List<string>();
            var bList = new List<string>();

            aList.Add(a.TaxonomyVersion);
            bList.Add(b.TaxonomyVersion);

            var differences = aList.ContentCompareReport(bList);

            return differences.Item1.Select(item => "(a) taxonomy-version: " + item).
                Concat(differences.Item2.Select(item => "(b) taxonomy-version: " + item));   
        }

        static IEnumerable<string> SchemaReferenceComparison(Instance a, Instance b)
        {
            var aList = new List<SchemaReference>();
            var bList = new List<SchemaReference>();

            aList.Add(a.SchemaReference);
            bList.Add(b.SchemaReference);

            var differences = aList.ContentCompareReport(bList);

            return differences.Item1.Select(item => "(a) " + item).
                Concat(differences.Item2.Select(item => "(b) " + item));   
        }


        static IEnumerable<string> FilingIndicatorComparison(Instance a, Instance b)
        {
            var differences = a.FilingIndicators.Where(fi => fi.Filed).ToList().
                ContentCompareReport(b.FilingIndicators.Where(fi => fi.Filed).ToList());

            var result = new List<string>(differences.Item1.Count + differences.Item2.Count);
            result.AddRange(differences.Item1.Select(item => "(a) " + item));
            result.AddRange(differences.Item2.Select(item => "(b) " + item));
            return result;
        }

        #endregion
    }
}

