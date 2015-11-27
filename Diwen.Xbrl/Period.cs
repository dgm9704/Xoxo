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
	[XmlRoot(ElementName = "period", Namespace = "http://www.xbrl.org/2003/instance")]
	public class Period : IEquatable<Period>
	{
		[XmlElement(ElementName = "instant", DataType = "date", Namespace = "http://www.xbrl.org/2003/instance")]
		public DateTime Instant { get; set; }

		public Period()
		{

		}

		public Period(DateTime instant)
			: this()
		{
			this.Instant = instant;
		}

		public Period(int year, int month, int day)
			: this()
		{
			this.Instant = new DateTime(year, month, day);
		}

		public override bool Equals(object obj)
		{
			var other = obj as Period;
			if(other != null)
			{
				return this.Equals(other);
			}
			else
			{
				return base.Equals(obj);
			}
		}

		#region IEquatable implementation

		public bool Equals(Period other)
		{
			return other != null && this.Instant.Equals(other.Instant);
		}

		public override int GetHashCode()
		{
			return this.Instant.GetHashCode();
		}

		#endregion
	}
}