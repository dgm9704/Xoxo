//namespace Xoxo
//{
//	using System;
//	using System.Xml.Serialization;
//	using System.Collections.ObjectModel;
//
//	public class MemberCollection<T>  : Collection<Member>, IEquatable<MemberCollection<T>>
//	{
//		public void Add(string dimension, string value)
//		{
//			base.Add(new Member(dimension, value));
//		}
//
//		#region IEquatable implementation
//
//		public bool Equals(Collection<Member> other)
//		{
//			var result = true;
//
//			if(this.Count != other.Count)
//			{
//				result = false;
//			}
//			else
//			{
//				for(int i = 0; i < this.Count; i++)
//				{
//					if(!this[i].Equals(other[i]))
//					{
//						result = false;
//						break;
//					}
//				}
//			}
//
//			return result;
//		}
//
//		#endregion
//	}
//}