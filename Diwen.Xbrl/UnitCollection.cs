//
//  This file is part of Diwen.Xbrl.
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System;
    using System.Linq;

    public class UnitCollection : KeyedCollection<string, Unit>, IEquatable<IList<Unit>>
    {
        Instance Instance;

        public UnitCollection()
        {

        }

        public UnitCollection(Instance instance)
            : this()
        {
            Instance = instance;
        }

        public void Add(string id, string measure)
        {
            base.Add(new Unit(id, measure));
        }

        public UnitCollection UsedUnits()
        {
            var result = new UnitCollection();
            foreach(var unit in this)
            {
                var fact = Instance.Facts.FirstOrDefault(f => f.Unit == unit);
                if(fact != null)
                {
                    result.Add(unit);
                }
            }

            return result;
        }

        protected override string GetKeyForItem(Unit item)
        {
            string key = null;
            if(item != null)
            {
                key = item.Id;
            }
            return key;
        }

        public override bool Equals(object obj)
        {
            var other = obj as UnitCollection;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach(var u in this)
            {
                hashCode = 31 * hashCode + u.GetHashCode();
            }
            return hashCode;
        }

        #region IEquatable implementation

        public bool Equals(IList<Unit> other)
        {
            return this.ContentCompare(other);
        }

        #endregion
    }
}