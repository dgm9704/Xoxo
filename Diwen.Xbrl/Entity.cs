//
//  This file is part of Diwen.xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2020 John Nordberg
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
	using System.Linq;
	using System.Xml.Serialization;

	[Serializable]
	public class Entity : IEquatable<Entity>
	{
		Instance instanceField;

		[XmlIgnore]
		public Instance Instance
		{
			get { return instanceField; }
			set
			{
				instanceField = value;
				Segment.Instance = value;
			}
		}

		[XmlElement("identifier", Namespace = "http://www.xbrl.org/2003/instance")]
		public Identifier Identifier { get; set; }

		[XmlElement("segment", Namespace = "http://www.xbrl.org/2003/instance")]
		public Segment Segment { get; set; }

		public bool ShouldSerializeSegment()
		{
			var result = false;
			if (Segment != null)
			{
				result = (Segment.ExplicitMembers.Any());
			}
			return result;
		}

		public Entity()
		{
			Identifier = new Identifier();
			Segment = new Segment();
		}

		public Entity(string identifierScheme, string identifierValue)
			: this()
		{
			Identifier = new Identifier(identifierScheme, identifierValue);
		}

		public ExplicitMember AddExplicitMember(string dimension, string value)
		=> Segment.ExplicitMembers.Add(dimension, value);

		public override string ToString()
		=> $"Identifier={Identifier}";

		#region IEquatable implementation

		public override int GetHashCode()
		=> Identifier.GetHashCode() + 7 * Segment.GetHashCode();


		public override bool Equals(object obj)
		=> Equals(obj as Entity);

		public bool Equals(Entity other)
		{
			var result = false;
			if (other != null)
			{
				if (Identifier.Equals(other.Identifier))
				{
					result |= (Segment == null && other.Segment == null)
						|| (Segment != null && Segment.Equals(other.Segment));
				}
			}
			return result;
		}

		#endregion
	}
}