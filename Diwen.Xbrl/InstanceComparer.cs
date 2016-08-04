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
using System.Collections.ObjectModel;

namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class InstanceComparer
    {
        public static ComparisonReport Report(Instance a, Instance b)
        {
            return Report(a, b, ComparisonTypes.All);
        }

        public static ComparisonReport Report(Instance a, Instance b, ComparisonTypes comparisonTypes)
        {
            var messages = new List<string>();

            foreach(var comparison in ComparisonMethods.Where(c=> comparisonTypes.HasFlag(c.Key)))
            {
                messages.AddRange(comparison.Value(a, b));
            }

            return new ComparisonReport(!messages.Any(), messages);
        }

        static Dictionary<ComparisonTypes, Func<Instance, Instance, List<string>>> ComparisonMethods
        = new Dictionary<ComparisonTypes, Func<Instance, Instance, List<string>>> {
            { ComparisonTypes.Basic, BasicComparison },
            { ComparisonTypes.Contexts, ScenarioComparison },
            { ComparisonTypes.Facts, FactComparison },
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


        static List<string> BasicComparison(Instance a, Instance b)
        {
            var result = new List<string>();
            foreach(var check in SimpleCheckMethods)
            {
                if(!check.Value(a, b))
                {
                    result.Add(check.Key);
                }
            }
            return result;
        }

        static bool CheckNullInstances(object a, object b)
        {
            return (a != null && b != null);
        }

        static bool CheckTaxonomyVersion(Instance a, Instance b)
        { 
            var result = false;
			
            if(a.TaxonomyVersion != null && b.TaxonomyVersion != null)
            {
                result = a.TaxonomyVersion.Equals(b.TaxonomyVersion, StringComparison.Ordinal);				
            }
            else
            {
                result |= a.TaxonomyVersion == null && b.TaxonomyVersion == null;
            }
            return result;
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
            bool result;

            if(a == null ^ b == null)
            {
                result = false;
            }
            else
            {
                if(a == null && b == null)
                {
                    result = true;
                }
                else
                {
                    result = a.Count == b.Count;
                }
            }
            return result;
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
            var aused = a.GetUsedDomainNamespaces();
            var bused = b.GetUsedDomainNamespaces();
            return aused.ContentCompare(bused);
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

            return (entityA == null && entityB == null) || (entityA != null && entityA.Equals(entityB));
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

            return (periodA == null && periodB == null) || (periodA != null && periodA.Equals(periodB));
        }

        #endregion

        #region DetailedChecks

        static List<string> ContextComparison(Instance a, Instance b)
        {
            var messages = new List<string>();
            var differences = a.Contexts.ContentCompareReport(b.Contexts);

            var notInB = differences.Item1;
            var notInA = differences.Item2;

            foreach(var item in notInB)
            {
                messages.Add("(a) " + item.Id + ":" + (item.Scenario != null ? item.Scenario.ToString() : String.Empty));
            }

            foreach(var item in notInA)
            {
                messages.Add("(b) " + item.Id + ":" + (item.Scenario != null ? item.Scenario.ToString() : String.Empty));
            } 

            return messages;
        }

        static List<string> ScenarioComparison(Instance a, Instance b)
        {
            var messages = new List<string>();

            var aList = new List<Scenario>();

            foreach(var c in a.Contexts)
            {
                aList.Add(c.Scenario);
            }

            var bList = new List<Scenario>();
            foreach(var c in b.Contexts)
            {
                bList.Add(c.Scenario);
            }

            var differences = aList.ContentCompareReport(bList);

            var notInB = differences.Item1;
            var notInA = differences.Item2;

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
                    var contextId = aLookup[item.ToString()];
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
                    var contextId = bLookup[item.ToString()];
                    messages.Add(string.Format("(b) {0}: {1}", contextId, item));
                }
            }
            return messages;
        }

        static List<string> FactComparison(Instance a, Instance b)
        {
            var messages = new List<string>();

            var differences = a.Facts.ContentCompareReport(b.Facts);

            var notInB = differences.Item1;
            var notInA = differences.Item2;

            foreach(var item in notInB)
            {
                var difference = "(a) " + item;
                messages.Add(difference);
            }

            foreach(var item in notInA)
            {
                var difference = "(b) " + item;
                messages.Add(difference);
            }

            return messages;
        }

        #endregion
    }
}

