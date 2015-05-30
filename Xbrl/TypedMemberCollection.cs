namespace Xoxo
{
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Xml.Serialization;

	public class TypedMemberCollection : Collection<TypedMember>, IEquatable<TypedMemberCollection>
	{
		private XbrlInstance Instance;

		public TypedMemberCollection(XbrlInstance instance)
		{
			this.Instance = instance;
		}

		public TypedMember Add(string dimension, string domain, string value)
		{
			var typedMember = new TypedMember(dimension, domain, value);
			base.Add(typedMember);
			return typedMember;
		}

		#region IEquatable implementation

		public bool Equals(TypedMemberCollection other)
		{
			return this.SequenceEqual(other);
		}

		#endregion
	}
}