//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2018 John Nordberg
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
	using System.Linq;

	public class UnitCollection : KeyedCollection<string, Unit>, IEquatable<IList<Unit>>
	{
		Instance Instance;

		public UnitCollection() { }

		public UnitCollection(Instance instance)
			: this()
		{
			Instance = instance;
		}

		public UnitCollection(IEnumerable<Unit> units)
		{
			AddRange(units);
		}

		public void AddRange(IEnumerable<Unit> units)
		{
			foreach (var unit in units)
			{
				Add(unit);
			}
		}

		public void Add(string id, string measure)
		=> Add(new Unit(id, measure));

		public UnitCollection UsedUnits()
		=> new UnitCollection(this.Where(u => Instance.Facts.Any(f => f.Unit == u)));

		protected override string GetKeyForItem(Unit item)
		=> item != null ? item.Id : null;

		public override bool Equals(object obj)
		=> Equals(obj as UnitCollection);

		public override int GetHashCode()
		{
			int hashCode = 0;
			foreach (var u in this)
			{
				hashCode = 31 * hashCode + u.GetHashCode();
			}
			return hashCode;
		}

		#region IEquatable implementation

		public bool Equals(IList<Unit> other)
		=> this.ContentCompare(other);

		#endregion
	}
}