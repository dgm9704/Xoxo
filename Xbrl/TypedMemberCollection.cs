namespace Xoxo
{
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Xml.Serialization;

	public class TypedMemberCollection : Collection<TypedMember>, IEquatable<TypedMemberCollection>
	{
		public void Add(string dimension, string domain, string value)
		{
			base.Add(new TypedMember(dimension, domain, value));
		}

		#region IEquatable implementation

		public bool Equals(TypedMemberCollection other)
		{
			return this.SequenceEqual(other);
		}

		#endregion
	}
}