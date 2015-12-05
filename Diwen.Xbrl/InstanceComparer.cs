//
//  This file is part of Diwen.xbrl.
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
using System.Collections.ObjectModel;

namespace Diwen.Xbrl
{
	using System;
	using System.Collections.Generic;

	public static class InstanceComparer
	{
		public static InstanceCompareReport Report(Instance a, Instance b)
		{
			var result = true;
			var messages = new List<string>();

			foreach(var check in SimpleCheckMethods)
			{
				if(!check.Value(a, b))
				{
					result = false;
					messages.Add(check.Key);
				}
			}

			return new InstanceCompareReport(result, messages);
		}

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

		private static bool CheckNullInstances(Instance a, Instance b)
		{
			return (a != null && b != null);
		}

		private static bool CheckTaxonomyVersion(Instance a, Instance b)
		{
			return a.TaxonomyVersion.Equals(b.TaxonomyVersion);
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
	}
}

