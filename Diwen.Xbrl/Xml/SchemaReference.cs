//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2024 John Nordberg
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

namespace Diwen.Xbrl.Xml
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "schemaRef", Namespace = "http://www.xbrl.org/2003/linkbase")]
    public class SchemaReference : IEquatable<SchemaReference>
    {
        [XmlAttribute("type", Namespace = "http://www.w3.org/1999/xlink")]
        public string Type { get; set; }

        [XmlAttribute("href", Namespace = "http://www.w3.org/1999/xlink")]
        public string Value { get; set; }

        public SchemaReference()
        {
        }

        public SchemaReference(string type, string value)
            : this()
        {
            Type = type;
            Value = value;
        }

        public override bool Equals(object obj)
        => Equals(obj as SchemaReference);

        public override string ToString()
        => $"SchemaReference: Type={Type}, Value={Value}";

        #region IEquatable implementation

        public bool Equals(SchemaReference other)
        => other != null
        && (Type == null ? other.Type == null : other.Type != null && Type.Equals(other.Type, StringComparison.Ordinal))
        && (Value == null ? other.Value == null : other.Value != null && Value.Equals(other.Value, StringComparison.Ordinal));

        public override int GetHashCode()
        => Value != null ? Value.GetHashCode() : 0;

        #endregion

    }
}