//
//  This file is part of Diwen.xbrl.
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

namespace Diwen.Xbrl.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Diwen.Xbrl.Extensions;

    /// <summary/>
    public class FactCollection : Collection<Fact>, IEquatable<IList<Fact>>
    {
        /// <summary/>
        public Report Report;

        /// <summary/>
        public FactCollection(Report report)
        {
            Report = report;
        }

        /// <summary/>
        public Fact Add(Context context, string metric, string unitRef, string decimals, string value)
        {
            var prefix = string.Empty;
            var ns = string.Empty;
            var idx = metric.IndexOf(':');
            if (idx != -1)
            {
                prefix = metric.Substring(0, idx);
                metric = metric.Substring(idx + 1);
            }

            if (!string.IsNullOrEmpty(prefix))
                ns = Report.Namespaces.LookupNamespace(prefix);

            if (string.IsNullOrEmpty(ns))
            {
                ns = Report.FactNamespace;
                prefix = Report.Namespaces.LookupPrefix(ns);
            }

            Unit unit = null;
            if (!string.IsNullOrEmpty(unitRef))
            {
                if (!Report.Units.Contains(unitRef))
                    throw new KeyNotFoundException($"Referenced unit '{unitRef}' does not exist");

                unit = Report.Units[unitRef];
            }

            var fact = new Fact(context, metric, unit, decimals, value, ns, prefix);
            Add(fact);
            return fact;
        }

        /// <summary/>
        public Fact Add(Scenario scenario, string metric, string unitRef, string decimals, string value)
        => Add(Report.GetContext(scenario), metric, unitRef, decimals, value);

        /// <summary/>
        public Fact Add(Segment segment, string metric, string unitRef, string decimals, string value)
        => Add(Report.GetContext(segment), metric, unitRef, decimals, value);

        /// <summary/>
        public void AddRange(IEnumerable<Fact> facts)
        {
            foreach (var fact in facts)
                Add(fact);
        }

        #region IEquatable implementation

        /// <summary/>
        public bool Equals(IList<Fact> other)
        => this.ContentCompare(other);

        #endregion
    }
}