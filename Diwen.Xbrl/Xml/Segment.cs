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
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "segment", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Segment : IEquatable<Segment>
    {
        Report report;

        [XmlIgnore]
        public Report Report
        {
            get => report;
            set
            {
                report = value;
                ExplicitMembers.Report = value;
                TypedMembers.Report = value;
            }
        }

        [XmlElement("explicitMember", Namespace = "http://xbrl.org/2006/xbrldi")]
        public ExplicitMemberCollection ExplicitMembers { get; set; }

        [XmlElement("typedMember", Namespace = "http://xbrl.org/2006/xbrldi")]
        public TypedMemberCollection TypedMembers { get; set; }

        public bool HasMembers => ExplicitMembers.Any() || TypedMembers.Any();

        public Segment()
        {
            ExplicitMembers = [];
            TypedMembers = [];
        }

        public Segment(Report report)
        {
            ExplicitMembers = new ExplicitMemberCollection(report);
            TypedMembers = new TypedMemberCollection(report);
        }

        public override string ToString()
        {
            var members = new List<string>();

            if (ExplicitMembers != null)
                members.AddRange(ExplicitMembers.Select(m => m.ToString()));

            if (TypedMembers != null)
                members.AddRange(TypedMembers.Select(m => m.ToString()));

            members.Sort();
            return string.Join(", ", members);
        }

        public override bool Equals(object obj)
        => Equals(obj as Segment);

        public override int GetHashCode()
        => TypedMembers.GetHashCode() + 31 * ExplicitMembers.GetHashCode();

        public ExplicitMember AddExplicitMember(string dimension, string value)
        => ExplicitMembers.Add(dimension, value);

        public TypedMember AddTypedMember(string dimension, string domain, string value)
        => TypedMembers.Add(dimension, domain, value);

        #region IEquatable implementation

        public bool Equals(Segment other)
        => other != null
            && ExplicitMembers.Equals(other.ExplicitMembers)
            && TypedMembers.Equals(other.TypedMembers);

        #endregion
    }
}