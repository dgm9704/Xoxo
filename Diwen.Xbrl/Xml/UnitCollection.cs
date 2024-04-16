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

namespace Diwen.Xbrl.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml;
    using Diwen.Xbrl.Extensions;

    public class UnitCollection : KeyedCollection<string, Unit>, IEquatable<IList<Unit>>
    {
        Report Report;

        public UnitCollection() { }

        public UnitCollection(Report Report)
            : this()
        {
            this.Report = Report;
        }

        public UnitCollection(IEnumerable<Unit> units)
        {
            AddRange(units);
        }

        public void AddRange(IEnumerable<Unit> units)
        {
            foreach (var unit in units)
            {
                unit.Report = this.Report;
                Add(unit);
            }
        }

        public void Add(string id, string value)
        {
            var idx = value.IndexOf(':');
            var prefix = value.Substring(0, idx);
            var ns = Report.Namespaces.LookupNamespace(prefix);
            var localname = value.Substring(idx + 1);
            var measure = new XmlQualifiedName(localname, ns);
            var unit = new Unit(id, measure);
            unit.Report = this.Report;
            Add(unit);
        }

        public UnitCollection UsedUnits()
        => new UnitCollection(this.Where(u => Report.Facts.Any(f => f.Unit == u)));

        protected override string GetKeyForItem(Unit item)
        => item != null ? item.Id : null;

        public override bool Equals(object obj)
        => Equals(obj as UnitCollection);

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (var u in this)
                hashCode = 31 * hashCode + u.GetHashCode();

            return hashCode;
        }

        #region IEquatable implementation

        public bool Equals(IList<Unit> other)
        => this.ContentCompare(other);

        #endregion
    }
}