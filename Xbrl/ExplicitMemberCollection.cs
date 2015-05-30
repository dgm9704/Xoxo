namespace Xoxo
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Xml.Serialization;

	public class ExplicitMemberCollection : Collection<ExplicitMember>, IEquatable<ExplicitMemberCollection>
	{
		private XbrlInstance Instance;

		public ExplicitMemberCollection(XbrlInstance instance)
		{
			this.Instance = instance;
		}

		public ExplicitMember Add(string dimension, string value)
		{
			if(!dimension.StartsWith(Instance.DimensionPrefix))
			{
				dimension = Instance.DimensionPrefix + ":" + dimension;
			}
			var explicitMember = new ExplicitMember(dimension, value);
			base.Add(explicitMember);
			return explicitMember;
		}


		#region IEquatable implementation

		public bool Equals(ExplicitMemberCollection other)
		{
			return this.SequenceEqual(other);
		}

		#endregion
	}
}