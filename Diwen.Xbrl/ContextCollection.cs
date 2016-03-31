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

namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;

    public class ContextCollection : KeyedCollection<string, Context>, IEquatable<IList<Context>>
    {
        static IFormatProvider ic = CultureInfo.InvariantCulture;

        Instance Instance;

        public string IdFormat { get; set; }

        public ContextCollection()
        {
            IdFormat = "A{0}";
        }

        public ContextCollection(Instance instance)
            : this()
        {
            Instance = instance;
        }

        public new Context Add(Context context)
        {
            if(context == null)
            {
                throw new ArgumentNullException("context");
            }

            if(context.Entity == null)
            {
                context.Entity = Instance.Entity;
            }

            if(context.Period == null)
            {
                context.Period = Instance.Period;
            }

            if(string.IsNullOrEmpty(context.Id))
            {
                var exists = false;
                foreach(var oldContext in this)
                {
                    if(context.Equals(oldContext))
                    {
                        exists = true;
                        context = oldContext;
                        break;
                    }
                }

                if(!exists)
                {
                    context.Id = NextId();
                    base.Add(context);
                }
            }
            else
            {
                base.Add(context);
            }

            return context;
        }

        public string NextId()
        {
            var counter = Count;
            string id;
            do
            {
                id = string.Format(ic, IdFormat, counter++);
            }
            while (Contains(id));

            return id;
        }

        public void AddRange(IEnumerable<Context> values)
        {
            if(values != null)
            {
                foreach(var item in values)
                {
                    Add(item);
                }
            }
        }

        protected override string GetKeyForItem(Context item)
        {
            string key = null;
            if(item != null)
            {
                key = item.Id;
            }
            return key;
        }

        #region IEquatable implementation

        public bool Equals(IList<Context> other)
        {
            return this.ContentCompare(other);
        }

        #endregion
    }
}