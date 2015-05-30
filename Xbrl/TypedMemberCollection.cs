namespace Xoxo
{
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Xml.Serialization;

	public class TypedMemberCollection : Collection<TypedMember>, IEquatable<TypedMemberCollection>
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

					if(!item.Domain.StartsWith(instance.TypedDomainPrefix))
					{
						item.Domain = Instance.TypedDomainPrefix + ":" + item.Domain;
					}
				}
			}
		}

		public TypedMemberCollection()
		{
			
		}

		public TypedMemberCollection(XbrlInstance instance) : this()
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