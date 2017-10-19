//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2017 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.Xbrl
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml.Serialization;

	[Serializable]
	[XmlRoot(ElementName = "scenario", Namespace = "http://www.xbrl.org/2003/instance")]
	public class Scenario : IEquatable<Scenario>
	{
		Instance instance;

		[XmlIgnore]
		public Instance Instance
		{
			get { return instance; }
			set
			{
				instance = value;
				ExplicitMembers.Instance = value;
				TypedMembers.Instance = value;
			}
		}

		[XmlElement("explicitMember", Namespace = "http://xbrl.org/2006/xbrldi")]
		public ExplicitMemberCollection ExplicitMembers { get; set; }

		[XmlElement("typedMember", Namespace = "http://xbrl.org/2006/xbrldi")]
		public TypedMemberCollection TypedMembers { get; set; }

		[XmlIgnore]
		public bool HasMembers
		=> ExplicitMembers.Any() || TypedMembers.Any();

		public Scenario()
		{
			ExplicitMembers = new ExplicitMemberCollection();
			TypedMembers = new TypedMemberCollection();
		}

		public Scenario(Instance instance)
		{
			ExplicitMembers = new ExplicitMemberCollection(instance);
			TypedMembers = new TypedMemberCollection(instance);
		}

		public override string ToString()
		{
			var members = new List<string>();
			if (ExplicitMembers != null)
			{
				members.AddRange(ExplicitMembers.Select(m => m.ToString()));
			}
			if (TypedMembers != null)
			{
				members.AddRange(TypedMembers.Select(m => m.ToString()));
			}

			members.Sort();
			return string.Join(", ", members);
		}

		public override bool Equals(object obj)
		=> Equals(obj as Scenario);

		int hashCode = -1;
		public override int GetHashCode()
		{
			if (hashCode == -1)
			{
				hashCode = TypedMembers.GetHashCode() + 31 * ExplicitMembers.GetHashCode();
			}
			return hashCode;
		}

		public ExplicitMember AddExplicitMember(string dimension, string value)
		=> ExplicitMembers.Add(dimension, value);

		public TypedMember AddTypedMember(string dimension, string domain, string value)
		=> TypedMembers.Add(dimension, domain, value);

		#region IEquatable implementation

		public bool Equals(Scenario other)
		{
			var result = false;
			if (other != null)
			{
				if (ExplicitMembers.Equals(other.ExplicitMembers))
				{
					result |= TypedMembers.Equals(other.TypedMembers);
				}
			}

			return result;
		}

		#endregion
	}
}