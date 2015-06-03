namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;

    public static class IListExtensions
    {
        public static bool SmartCompare<T>(this IList<T> left, IList<T> right)
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
                    // if different number of items then not equal
                    if (left.Count != right.Count)
                    {
                        result = false;
                    }
                    else
                    {
                        // try to match each item from left to right 
                        var list = new List<T>(right);
                        for (int i = 0; i < left.Count; i++)
                        {
                            var candidate = left[i];
                            var idx = list.IndexOf(candidate);
                            if (idx == -1)
                            {
                                result = false;
                                break;
                            }
                            else
                            {
                                // if match found, remove from right to minimize unnecessary comparisons
                                list.RemoveAt(idx);
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}

