//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015 John Nordberg
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
	using System.Collections.ObjectModel;
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

		private delegate List<string> ComparisonMethod(Instance a, Instance b);

		private static Dictionary<ComparisonTypes, ComparisonMethod> ComparisonMethods
		= new Dictionary<ComparisonTypes, ComparisonMethod> {
			{ ComparisonTypes.Basic, BasicComparison },
			{ ComparisonTypes.Contexts, ContextComparison },
			{ ComparisonTypes.Facts, FactComparison },
		};

		#region SimpleChecks

		private delegate bool SimpleCheckMethod(Instance a, Instance b);

		private static Dictionary<string, SimpleCheckMethod> SimpleCheckMethods 
		= new Dictionary<string, SimpleCheckMethod> {
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


		private static List<string> BasicComparison(Instance a, Instance b)
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

		private static bool CheckNullInstances(Instance a, Instance b)
		{
			return (a != null && b != null);
		}

		private static bool CheckTaxonomyVersion(Instance a, Instance b)
		{ 
			var result = false;
			
			if(a.TaxonomyVersion != null && b.TaxonomyVersion != null)
			{
				result = a.TaxonomyVersion.Equals(b.TaxonomyVersion);				
			}
			else if(a.TaxonomyVersion == null && b.TaxonomyVersion == null)
			{
				result = true;
			}
			return result;
		}

		private static bool CheckSchemaReference(Instance a, Instance b)
		{
			return a.SchemaReference.Equals(b.SchemaReference);
		}

		private static bool CheckUnits(Instance a, Instance b)
		{
			return a.Units.Equals(b.Units);
		}

		private static bool CheckFilingIndicators(Instance a, Instance b)
		{
			return a.FilingIndicators.Equals(b.FilingIndicators);
		}

		private static bool CheckContextCount(Instance a, Instance b)
		{
			var result = true;

			if(a.Contexts == null ^ b.Contexts == null)
			{
				result = false;
			}
			else
			{
				if(a.Contexts == null && b.Contexts == null)
				{
					result = true;
				}
				else
				{
					result = a.Contexts.Count == b.Contexts.Count;
				}
			}
			return result;
		}

		private static bool CheckFactCount(Instance a, Instance b)
		{
			var result = true;

			if(a.Facts == null ^ b.Facts == null)
			{
				result = false;
			}
			else
			{
				if(a.Facts == null && b.Facts == null)
				{
					result = true;
				}
				else
				{
					result = a.Facts.Count == b.Facts.Count;
				}
			}
			return result;
		}

		private static bool CheckDomainNamespaces(Instance a, Instance b)
		{
			var aused = a.GetUsedDomainNamespaces();
			var bused = b.GetUsedDomainNamespaces();
			return aused.ContentCompare(bused);
		}

		private static bool CheckEntity(Instance a, Instance b)
		{
			Entity entityA = null;
			Entity entityB = null;
			if(a.Contexts != null && a.Contexts.Count != 0)
			{
				entityA = a.Contexts[0].Entity;

				if(b.Contexts != null && b.Contexts.Count != 0)
				{
					entityB = b.Contexts[0].Entity;
				}
			}

			return entityA.Equals(entityB);
		}

		private static bool CheckPeriod(Instance a, Instance b)
		{
			Period periodA = null;
			Period periodB = null;
			if(a.Contexts != null && a.Contexts.Count != 0)
			{
				periodA = a.Contexts[0].Period;

				if(b.Contexts != null && b.Contexts.Count != 0)
				{
					periodB = b.Contexts[0].Period;
				}
			}

			return periodA.Equals(periodB);
		}

		#endregion

		#region DetailedChecks

		private static List<string> ContextComparison(Instance a, Instance b)
		{
			var messages = new List<string>();
			var differences = a.Contexts.ContentCompareReport(b.Contexts);

			var notInB = differences.Item1;
			var notInA = differences.Item2;

			foreach(var item in notInB)
			{
				messages.Add("(a) " + item.Id + ":" + (item.Scenario != null ? item.Scenario.ToString() : ""));
			}

			foreach(var item in notInA)
			{
				messages.Add("(b) " + item.Id + ":" + (item.Scenario != null ? item.Scenario.ToString() : ""));
			} 

			return messages;
		}

		private static List<string> FactComparison(Instance a, Instance b)
		{
			var messages = new List<string>();

			var differences = a.Facts.ContentCompareReport(b.Facts);

			var notInB = differences.Item1;
			var notInA = differences.Item2;

			foreach(var item in notInB)
			{
				var difference = "(a) " + item.ToString();
				messages.Add(difference);
			}

			foreach(var item in notInA)
			{
				var difference = "(b) " + item.ToString();
				messages.Add(difference);
			}

			return messages;
		}

		#endregion
	}
}

