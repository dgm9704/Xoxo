//
//  This file is part of Diwen.xbrl.
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

    using System.Globalization;
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "explicitMember", Namespace = "http://xbrl.org/2006/xbrldi")]
    public class ExplicitMember : IEquatable<ExplicitMember>, IComparable<ExplicitMember>
    {
        static IFormatProvider ic = CultureInfo.InvariantCulture;

        internal Instance Instance { get; set; }

        [XmlAttribute("dimension", Namespace = "http://xbrl.org/2006/xbrldi")]
        public XmlQualifiedName Dimension { get; set; }

        [XmlText]
        public XmlQualifiedName Value { get; set; }

        public ExplicitMember()
        {
        }

        public ExplicitMember(XmlQualifiedName dimension, XmlQualifiedName value)
            : this()
        {
            Dimension = dimension;
            Value = value;
        }

        public string MemberCode
        {
            get
            {
                var prefix = Value.Prefix();
                var localname = Value.LocalName();

                if(string.IsNullOrEmpty(prefix))
                {
                    prefix = Instance.Namespaces.LookupPrefix(Value.Namespace);
                }

                return string.Join(":", prefix, localname);
            }
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ExplicitMember;
            return other != null && Equals(other);
        }

        public override string ToString()
        {
            return string.Format(ic, "{0}={1}", Dimension.LocalName(), MemberCode);
        }

        public int Compare(ExplicitMember other)
        {
            return CompareTo(other);
        }

        #region operator overloads

        public static bool operator ==(ExplicitMember left, ExplicitMember right)
        {
            bool result;

            // If both are null, or both are same instance, return true.
            if(object.ReferenceEquals(left, right))
            {
                result = true;
            }
            // If one is null, but not both, return false.
            else if(((object)left == null) || ((object)right == null))
            {
                result = false;
            }
            else
            {
                // Return true if the fields match:
                result = left.Equals(right);
            }

            return result;
        }

        public static bool operator !=(ExplicitMember left, ExplicitMember right)
        {
            bool result;

            // If one is null, but not both, return true.
            if(((object)left == null) || ((object)right == null))
            {
                result = true;
            }
            else
            {
                result = !left.Equals(right);
            }

            return result;
        }

        public static bool operator >(ExplicitMember left, ExplicitMember right)
        {
            bool result;

            // If both are null, or both are same instance, return false.
            if(object.ReferenceEquals(left, right))
            {
                result = false;
            }
            else
            {
                result = left != null && left.CompareTo(right) > 0;
            }

            return result;
        }

        public static bool operator <(ExplicitMember left, ExplicitMember right)
        {
            return right > left;
        }

        #endregion

        #region IEquatable implementation

        public bool Equals(ExplicitMember other)
        {
            var result = false;
            if(other != null)
            {
                if(Dimension == other.Dimension)
                {
                    result |= Value == other.Value;
                }

            }

            return result;
        }

        #endregion

        #region IComparable implementation

        public int CompareTo(ExplicitMember other)
        {
            int result;
            if(other == null)
            {
                result = 1;
            }
            else
            {
                result = string.Compare(Dimension.Name, other.Dimension.Name, StringComparison.OrdinalIgnoreCase);
                if(result == 0)
                {
                    result = string.Compare(Value.Name, other.Value.Name, StringComparison.OrdinalIgnoreCase);
                }
            }

            return result;
        }

        #endregion
    }
}