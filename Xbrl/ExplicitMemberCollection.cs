namespace Xoxo
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Xml.Serialization;

	public class ExplicitMemberCollection : Collection<ExplicitMember>, IEquatable<ExplicitMemberCollection>
	{
		public void Add(string dimension, string value)
		{
			base.Add(new ExplicitMember(dimension, value));
		}


		#region IEquatable implementation

		public bool Equals(ExplicitMemberCollection other)
		{
			return this.SequenceEqual(other);
		}

		#endregion
	}
}