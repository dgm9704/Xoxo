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
    using System.Globalization;
    using Diwen.Xbrl.Extensions;

    /// <summary/>
    public class ContextCollection : KeyedCollection<string, Context>, IEquatable<IList<Context>>
    {
        static readonly IFormatProvider ic = CultureInfo.InvariantCulture;

        Report report;

        /// <summary/>
        public string IdFormat { get; set; }

        /// <summary/>
        public ContextCollection()
        {
            IdFormat = "A{0}";
        }

        /// <summary/>
        public ContextCollection(Report report)
            : this()
        {
            this.report = report;
        }

        /// <summary/>
        public new Context Add(Context context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Entity == null)
                context.Entity = report.Entity;

            if (context.Period == null)
                context.Period = report.Period;

            if (string.IsNullOrEmpty(context.Id))
            {
                var exists = false;
                foreach (var oldContext in this)
                {
                    if (context.Equals(oldContext))
                    {
                        exists = true;
                        context = oldContext;
                        break;
                    }
                }

                if (!exists)
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

        /// <summary/>
        public string NextId()
        {
            var counter = Count;
            string id;
            do
                id = string.Format(ic, IdFormat, counter++);
            while (Contains(id));

            return id;
        }

        /// <summary/>
        public void AddRange(IEnumerable<Context> values)
        {
            if (values != null)
                foreach (var item in values)
                    Add(item);
        }

        /// <summary/>
        protected override string GetKeyForItem(Context item)
        => item?.Id;

        #region IEquatable implementation

        /// <summary/>
        public bool Equals(IList<Context> other)
        => this.ContentCompare(other);

        #endregion
    }
}