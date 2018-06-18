//
//  This file is part of Diwen.xbrl.
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

	public class FilingIndicatorCollection : Collection<FilingIndicator>, IEquatable<IList<FilingIndicator>>
	{
		public FilingIndicator Add(Context context, string value)
		=> Add(context, value, true);

		public FilingIndicator Add(Context context, string value, bool filed)
		{
			var filingIndicator = new FilingIndicator(context, value, filed);
			Add(filingIndicator);
			return filingIndicator;
		}

		#region IEquatable implementation

		public bool Equals(IList<FilingIndicator> other)
		{
			bool result;

			if (this == null && other == null)
			{
				// if both are null then consider equal
				result = true;
			}
			else if (this == null ^ other == null)
			{
				// if just one is null then not equal
				result = false;
			}
			else
			{
				result = this.
					Where(i => i.Filed).
					ToList().
					ContentCompare(other.
						Where(i => i.Filed).
						ToList());
			}
			return result;
		}

		#endregion
	}
}