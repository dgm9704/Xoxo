//
//  This file is part of Diwen.Xbrl.
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
	using System.Diagnostics;
	using System.Xml;
	using System.Xml.Serialization;

	[DebuggerDisplay("{Id}")]
	[Serializable]
	[XmlRoot(ElementName = "unit", Namespace = "http://www.xbrl.org/2003/instance")]
	public class Unit : IEquatable<Unit>, IXbrlObject
	{
		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlElement("measure")]
		public XmlQualifiedName Measure { get; set; }

		public Unit() { }

		public Unit(string id, string measure)
			: this()
		{
			Id = id;
			Measure = new XmlQualifiedName(measure);
		}

		public override string ToString()
		=> Measure.ToString();

		public override bool Equals(object obj)
		=> Equals(obj as Unit);

		#region IEquatable implementation

		public bool Equals(Unit other)
		=> other != null
			&& Measure.ToString().Equals(other.Measure.ToString(), StringComparison.Ordinal);

		public override int GetHashCode()
		=> Measure != null ? Measure.GetHashCode() : 0;

		public string ComparisonMessage()
		=> ToString();

		#endregion
	}
}