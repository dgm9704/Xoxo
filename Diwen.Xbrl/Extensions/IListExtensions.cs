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

namespace Diwen.Xbrl.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Diwen.Xbrl.Xml;

    public static class ListExtensions
    {
        internal static bool ContentCompare<T>(this IList<T> left, IList<T> right)
        {
            var result = true;

            // if both are null then consider equal
            if (left != null && right != null)
            {
                // if just one is null then not equal
                if (left == null ^ right == null)
                {
                    result = false;
                }
                else
                {
                    var leftCount = left.Count;
                    var rightCount = right.Count;

                    // if different number of items then not equal
                    if (leftCount != rightCount)
                    {
                        result = false;
                    }
                    else
                    {
                        // try to match each item from left to right
                        var list = new LinkedList<T>(right);
                        for (int i = 0; i < leftCount; i++)
                        {
                            var match = list.Find(left[i]);
                            if (match == null)
                            {
                                result = false;
                                break;
                            }

                            // if match found, remove from right to minimize unnecessary comparisons
                            list.Remove(match);

                        }
                    }
                }
            }
            return result;
        }

        internal static Tuple<List<T>, List<T>> ContentCompareReport<T>(this IList<T> left, IList<T> right)
        => new Tuple<List<T>, List<T>>(
                left.AsParallel().Except(right.AsParallel()).ToList(),
                right.AsParallel().Except(left.AsParallel()).ToList());

        internal static void RemoveUnusedItems<T>(this IList<T> items, ICollection<string> usedIds) where T : class, IXbrlObject
        {
            for (int i = 0; i < items.Count; i++)
            {
                var c = items[i];
                if (!usedIds.Contains(c.Id))
                    items[i] = null;
            }

            T nullValue = null;
            while (items.Remove(nullValue)) { }
        }

    }
}