//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

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