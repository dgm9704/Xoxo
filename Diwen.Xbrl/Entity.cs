//
//  This file is part of Diwen.xbrl.
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
	public class Entity : IEquatable<Entity>
	{
		[XmlElement("identifier", Namespace = "http://www.xbrl.org/2003/instance")]
		public Identifier Identifier { get; set; }

		public Entity()
		{
		}

		public Entity(string identifierScheme, string identifierValue)
			: this()
		{
			this.Identifier = new Identifier(identifierScheme, identifierValue);
		}

		#region IEquatable implementation

		public bool Equals(Entity other)
		{
			return other != null && this.Identifier.Equals(other.Identifier);
		}

		#endregion
	}
}