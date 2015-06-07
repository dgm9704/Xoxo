namespace Diwen.Xbrl
{
	using System;
	using System.Collections.Generic;

	public static class ListExtensions
	{
		public static bool SmartCompare<T>(this IList<T> left, IList<T> right)
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
	}
}

