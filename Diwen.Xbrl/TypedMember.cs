//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2016 John Nordberg
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
	using System.Xml;
	using System.Xml.Schema;
	using System.Xml.Serialization;

	public class TypedMember : IXmlSerializable, IEquatable<TypedMember>, IComparable<TypedMember>
	{
		internal Instance Instance { get; set; }

		[XmlIgnore]
		public XmlQualifiedName Dimension { get; set; }

		[XmlIgnore]
		public XmlQualifiedName Domain { get; set; }

		[XmlIgnore]
		public string Value { get; set; }

		public TypedMember()
		{

		}

		public TypedMember(XmlQualifiedName dimension, XmlQualifiedName domain, string value)
			: this()
		{
			Dimension = dimension;
			Domain = domain;
			Value = value;
		}

		public override string ToString()
		{
			return $"{Dimension.LocalName()}={Value}";
		}

		public override bool Equals(object obj)
		{
			var other = obj as TypedMember;
			return other != null && Equals(other);
		}

		public override int GetHashCode()
		{
			return Value != null ? Value.GetHashCode() : 0;
		}

		#region operator overloads

		public static bool operator ==(TypedMember left, TypedMember right)
		{
			// If both are null, or both are same instance, return true.
			if (ReferenceEquals(left, right))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)left == null) || ((object)right == null))
			{
				return false;
			}

			// Return true if the fields match:
			return left.Equals(right);
		}

		public static bool operator !=(TypedMember left, TypedMember right)
		{
			// If one is null, but not both, return true.
			if (((object)left == null) || ((object)right == null))
			{
				return true;
			}
			return !left.Equals(right);
		}

		public static bool operator >(TypedMember left, TypedMember right)
		{
			// If both are null, or both are same instance, return false.
			if (ReferenceEquals(left, right))
			{
				return false;
			}
			return left != null && left.CompareTo(right) > 0;
		}

		public static bool operator <(TypedMember left, TypedMember right)
		{
			return right > left;
		}

		public int Compare(TypedMember other)
		{
			return CompareTo(other);
		}

		#endregion

		#region IXmlSerializable implementation

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException(nameof(reader));
			}
			reader.MoveToContent();
			var dim = reader.GetAttribute("dimension");
			var idx = dim.IndexOf(':');
			var prefix = dim.Substring(0, idx);
			var dimNs = reader.LookupNamespace(prefix);
			Dimension = new XmlQualifiedName(dim, dimNs);
			reader.ReadStartElement();
			Domain = new XmlQualifiedName(reader.Name, reader.NamespaceURI);
			Value = reader.ReadElementString();
			reader.ReadEndElement();

		}

		public void WriteXml(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}
			writer.WriteAttributeString("dimension", Dimension.Name);

			if (!string.IsNullOrEmpty(Value))
			{
				writer.WriteElementString(Domain.Prefix(), Domain.LocalName(), Domain.Namespace, Value);
			}
			else
			{
				writer.WriteStartElement(Domain.Prefix(), Domain.LocalName(), Domain.Namespace);
				writer.WriteAttributeString("xsi", "nil", XmlSchema.InstanceNamespace, "true");
				writer.WriteEndElement();
			}

		}

		#endregion

		#region IEquatable implementation

		public bool Equals(TypedMember other)
		{
			return other != null
			&& Dimension == other.Dimension
			&& Domain == other.Domain
			&& Value == other.Value;
		}

		#endregion

		#region IComparable implementation

		public int CompareTo(TypedMember other)
		{
			int result;
			if (other == null)
			{
				result = -1;
			}
			else
			{
				result = string.Compare(Dimension.Name, other.Dimension.Name, StringComparison.OrdinalIgnoreCase);
				if (result == 0)
				{
					result = string.Compare(Domain.Name, other.Domain.Name, StringComparison.OrdinalIgnoreCase);
				}
				if (result == 0)
				{
					result = string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);
				}
			}
			return result;
		}

		#endregion
	}
}