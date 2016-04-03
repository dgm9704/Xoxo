//
//  This file is part of Diwen.Xbrl.
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

    public static class ListExtensions
    {

        public static bool ContentCompareEx<T>(this IList<T> left, IList<T> right)
        {
            var result = true;

            // if both are null then consider equal
            if(left != null && right != null)
            {
                // if just one is null then not equal
                if(left == null ^ right == null)
                {
                    result = false;
                }
                else
                {
                    var leftCount = left.Count;
                    var rightCount = right.Count;

                    // if different number of items then not equal
                    if(leftCount != rightCount)
                    {
                        result = false;
                    }
                    else
                    {
                        // try to match each item from left to right
                        var list = new HashSet<T>(right);
                        for(int i = 0; i < leftCount; i++)
                        {
                            if(!list.Remove(left[i]))
                            {
                                result = false;
                                break; 
                            }
                        }
                    }
                }
            }
            return result;
        }

        internal static Tuple<List<T>, List<T>> ContentCompareReportEx<T>(this IList<T> left, IList<T> right)
        {
            var notInB = new List<T>();
            var notInATmp = new HashSet<T>();
            if(left != null && right != null)
            {
                var leftCount = left.Count;
                var rightCount = right.Count;

                // try to match each item from left to right
                notInATmp = new HashSet<T>(right);
                for(int i = 0; i < leftCount; i++)
                {
                    var candidate = left[i];

                    if(!notInATmp.Remove(candidate))
                    {
                        notInB.Add(candidate);
                    }
                }
            }

            var notInA = new List<T>(notInATmp);
            return new Tuple<List<T>, List<T>>(notInB, notInA);

        }

        public static bool ContentCompare<T>(this IList<T> left, IList<T> right)
        {
            var result = true;
		
            // if both are null then consider equal
            if(left != null && right != null)
            {
                // if just one is null then not equal
                if(left == null ^ right == null)
                {
                    result = false;
                }
                else
                {
                    var leftCount = left.Count;
                    var rightCount = right.Count;
		
                    // if different number of items then not equal
                    if(leftCount != rightCount)
                    {
                        result = false;
                    }
                    else
                    {
                        // try to match each item from left to right
                        var list = new LinkedList<T>(right);
                        for(int i = 0; i < leftCount; i++)
                        {
                            var match = list.Find(left[i]);
                            if(match == null)
                            {
                                result = false;
                                break;
                            }
                            else
                            {
                                // if match found, remove from right to minimize unnecessary comparisons
                                list.Remove(match);
                            }
                        }
                    }
                }
            }
            return result;
        }

        internal static Tuple<List<T>, List<T>> ContentCompareReport<T>(this IList<T> left, IList<T> right)
        {
            var notInB = new List<T>();
            var notInATmp = new LinkedList<T>();
            if(left != null && right != null)
            {
                var leftCount = left.Count;
                var rightCount = right.Count;

                // try to match each item from left to right
                notInATmp = new LinkedList<T>(right);
                for(int i = 0; i < leftCount; i++)
                {
                    var candidate = left[i];
                    var match = notInATmp.Find(candidate);
                    if(match == null)
                    {
                        notInB.Add(candidate);
                    }
                    else
                    {
                        // if match found, remove from right to minimize unnecessary comparisons
                        notInATmp.Remove(match);
                    }
                }
            }

            var notInA = new List<T>(notInATmp);
            return new Tuple<List<T>, List<T>>(notInB, notInA);

        }
    }
}

