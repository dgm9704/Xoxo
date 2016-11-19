//
//  This file is part of Diwen.xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2016 John Nordberg
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

namespace Diwen.Xbrl
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	public class FactCollection : Collection<Fact>, IEquatable<IList<Fact>>
	{
		public Instance Instance;

		public FactCollection(Instance instance)
		{
			Instance = instance;
		}

		public Fact Add(Context context, string metric, string unitRef, string decimals, string value)
		{
			var ns = Instance.FactNamespace;
			var prefix = Instance.Namespaces.LookupPrefix(ns);
			if (prefix == null && metric.Contains(":"))
			{
				prefix = metric.Substring(0, metric.IndexOf(':'));
				metric = metric.Substring(metric.IndexOf(':') + 1);
			}
			if (ns == null)
			{
				ns = Instance.Namespaces.LookupNamespace(prefix);
			}

			Unit unit = null;
			if (!string.IsNullOrEmpty(unitRef))
			{
				if (!Instance.Units.Contains(unitRef))
				{
					throw new KeyNotFoundException($"Referenced unit '{unitRef}' does not exist");
				}
				unit = Instance.Units[unitRef];
			}

			var fact = new Fact(context, metric, unit, decimals, value, ns, prefix);
			Add(fact);
			return fact;
		}

		public Fact Add(Scenario scenario, string metric, string unitRef, string decimals, string value)
		{
			var context = Instance.GetContext(scenario);
			return Add(context, metric, unitRef, decimals, value);
		}

		public Fact Add(Segment segment, string metric, string unitRef, string decimals, string value)
		{
			var context = Instance.GetContext(segment);
			return Add(context, metric, unitRef, decimals, value);
		}

		public void AddRange(IEnumerable<Fact> facts)
		{
			foreach (var fact in facts)
			{
				Add(fact);
			}
		}

		#region IEquatable implementation

		public bool Equals(IList<Fact> other)
		{
			return this.ContentCompare(other);
		}

		#endregion
	}
}