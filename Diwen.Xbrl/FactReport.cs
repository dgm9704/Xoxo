//
//  This file is part of Diwen.xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2017 John Nordberg
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
	using System.Linq;

	public class FactReport
	{
		public List<Tuple<Fact, Fact>> Matches { get; set; } = new List<Tuple<Fact, Fact>>();

		public static FactReport FromReport(ComparisonReportObjects report)
		{
			var result = new FactReport();

			var l = new LinkedList<Fact>(report.Facts.Item1);
			var r = new LinkedList<Fact>(report.Facts.Item2);

			foreach (var left in l)
			{
				Fact right = null;
				foreach (var candidate in r)
				{
					var match = false;
					if (candidate.Metric.Equals(left.Metric) && candidate.Context.Scenario.Equals(left.Context.Scenario))
					{
						match = true;
					}
					else if (candidate.Equals(left))
					{
						match = true;
					}

					if (match)
					{
						right = candidate;
						r.Remove(candidate);
						break;
					}
				}

				if (right != null)
				{
					result.Matches.Add(Tuple.Create(left, right));
				}
			}

			return result;
		}
	}
}
