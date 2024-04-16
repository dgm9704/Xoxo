﻿//
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

namespace Diwen.Xbrl.Extensions
{
    using System.Collections.Generic;

    public static class IEnumerableOfStringExtensions
    {
        public static string Join(this IEnumerable<string> values, string separator)
        => values != null
            ? string.Join(separator ?? "", values)
            : string.Empty;

        public static HashSet<string> ToHashSet(this IEnumerable<string> values)
        => new HashSet<string>(values);
    }
}
