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
		private XbrlInstance instance;

		[XmlIgnore]
		public XbrlInstance Instance
		{
			get { return instance; }
			set
			{
				instance = value;
				foreach(var item in Items)
				{
					if(!item.Dimension.StartsWith(instance.DimensionPrefix))
					{
						item.Dimension = Instance.DimensionPrefix + ":" + item.Dimension;
					}
				}
			}
		}

		public ExplicitMemberCollection()
		{
		}

		public ExplicitMemberCollection(XbrlInstance instance) : this()
		{
			this.Instance = instance;
		}

		public ExplicitMember Add(string dimension, string value)
		{
			if(this.Instance != null && !dimension.StartsWith(this.Instance.DimensionPrefix))
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