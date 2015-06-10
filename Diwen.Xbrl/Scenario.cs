namespace Diwen.Xbrl
{
	using System;
	using System.Xml.Serialization;

	[Serializable]
	[XmlRoot(ElementName = "scenario", Namespace = "http://www.xbrl.org/2003/instance")]
	public class Scenario : IEquatable<Scenario>
	{
		private Instance instanceField;

		[XmlIgnore]
		public Instance Instance
		{
			get { return this.instanceField; }
			set
			{
				this.instanceField = value;
				this.ExplicitMembers.Instance = value;
				this.TypedMembers.Instance = value;
			}
		}

		[XmlElement("explicitMember", Namespace = "http://xbrl.org/2006/xbrldi")]
		public ExplicitMemberCollection ExplicitMembers { get; private set; }

		[XmlElement("typedMember", Namespace = "http://xbrl.org/2006/xbrldi")]
		public TypedMemberCollection TypedMembers { get; private set; }

		public Scenario()
		{
			this.ExplicitMembers = new ExplicitMemberCollection();
			this.TypedMembers = new TypedMemberCollection();
		}

		public Scenario(Instance instance)
		{
			this.ExplicitMembers = new ExplicitMemberCollection(instance);
			this.TypedMembers = new TypedMemberCollection(instance);
		}

		public override bool Equals(object obj)
		{
			var other = obj as Scenario;
			if(other != null)
			{
				return this.Equals(other);
			}
			else
			{
				return base.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			return (this.TypedMembers.Count * 1000) ^ this.ExplicitMembers.Count;
		}

		public ExplicitMember AddExplicitMember(string dimension, string value)
		{
			return this.ExplicitMembers.Add(dimension, value);
		}

		public TypedMember AddTypedMember(string dimension, string domain, string value)
		{
			return this.TypedMembers.Add(dimension, domain, value);
		}

		#region IEquatable implementation

		public bool Equals(Scenario other)
		{
			var result = false;
			if(other != null)
			{
				if(this.ExplicitMembers.Equals(other.ExplicitMembers))
				{
					if(this.TypedMembers.Equals(other.TypedMembers))
					{
						result = true;
					}
				}
			}

			return result;
		}

		#endregion
	}
}