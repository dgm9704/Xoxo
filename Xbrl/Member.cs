namespace Xoxo
{
	using System;
	using System.Xml.Serialization;
	using System.Collections.ObjectModel;

	public class Member : IMember, IEquatable<Member>
	{
		public virtual string Dimension		{ get; set; }

		public virtual string Value		{ get; set; }

		#region IEquatable implementation

		public bool Equals(Member other)
		{
			return this.Dimension == other.Dimension
			&& this.Value == other.Value;
		}

		#endregion
	}

}