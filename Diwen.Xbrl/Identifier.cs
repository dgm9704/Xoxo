//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2018 John Nordberg
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
	using System.Xml.Serialization;

	[Serializable]
	public class Identifier : IEquatable<Identifier>
	{
		[XmlAttribute("scheme", Namespace = "http://www.xbrl.org/2003/instance")]
		public string Scheme { get; set; }

		[XmlText]
		public string Value { get; set; }

		public Identifier()
		{
		}

		public Identifier(string scheme, string value)
			: this()
		{
			Scheme = scheme;
			Value = value;
		}

		public override string ToString()
		=> $"{Scheme}:{Value}";

		#region IEquatable implementation

		public bool Equals(Identifier other)
		=> other != null
			&& ((Scheme == null && other.Scheme == null) || Scheme.Equals(other.Scheme, StringComparison.Ordinal))
			&& ((Value == null && other.Value == null) || Value.Equals(other.Value, StringComparison.Ordinal));

		public override int GetHashCode()
		=> Scheme == null ? 0 : Scheme.GetHashCode() + 7 * (Value == null ? 0 : Value.GetHashCode());

		#endregion

		public override bool Equals(object obj)
		=> Equals(obj as Identifier);
	}
}