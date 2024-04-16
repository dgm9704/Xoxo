//
//  This file is part of Diwen.xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2020 John Nordberg
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

namespace Diwen.Xbrl.Xml.Comparison
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ContextReport
    {
        public List<Tuple<Scenario, Scenario>> Matches { get; set; } = new List<Tuple<Scenario, Scenario>>();

        public static ContextReport FromReport(ComparisonReportObjects report)
        {
            var result = new ContextReport();

            foreach (var left in report.Contexts.Item1)
            {
                Scenario right = null;
                var leftFacts = left.Instance.Facts.Where(f => f.Context.Scenario == left);

                foreach (var r in report.Contexts.Item2)
                {
                    var rightFacts = r.Instance.Facts.Where(f => f.Context.Scenario == r);
                    if (rightFacts.Count() == leftFacts.Count())
                    {
                        right = r;
                        break;
                    }
                }

                if (right != null)
                    result.Matches.Add(Tuple.Create(left, right));
            }

            return result;
        }
    }
}
