﻿//
//  This file is part of Diwen.xbrl.
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
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary/>
    [Serializable]
    public class Entity : IEquatable<Entity>
    {
        Report reportField;

        /// <summary/>
        [XmlIgnore]
        public Report Report
        {
            get { return reportField; }
            set
            {
                reportField = value;
                Segment.Report = value;
            }
        }

        /// <summary/>
        [XmlElement("identifier", Namespace = "http://www.xbrl.org/2003/instance")]
        public Identifier Identifier { get; set; }

        /// <summary/>
        [XmlElement("segment", Namespace = "http://www.xbrl.org/2003/instance")]
        public Segment Segment { get; set; }

        /// <summary/>
        public bool ShouldSerializeSegment()
        {
            var result = false;
            if (Segment != null)
                result = (Segment.ExplicitMembers.Any());

            return result;
        }

        /// <summary/>
        public Entity()
        {
            Identifier = new Identifier();
            Segment = new Segment();
        }

        /// <summary/>
        public Entity(string identifierScheme, string identifierValue)
            : this()
        {
            Identifier = new Identifier(identifierScheme, identifierValue);
        }

        /// <summary/>
        public ExplicitMember AddExplicitMember(string dimension, string value)
        => Segment.ExplicitMembers.Add(dimension, value);

        /// <summary/>
        public override string ToString()
        => $"Identifier={Identifier}";

        #region IEquatable implementation

        /// <summary/>
        public override int GetHashCode()
        => Identifier.GetHashCode() + 7 * Segment.GetHashCode();

        /// <summary/>
        public override bool Equals(object obj)
        => Equals(obj as Entity);

        /// <summary/>
        public bool Equals(Entity other)
        {
            var result = false;
            if (other != null)
                if (Identifier.Equals(other.Identifier))
                    result |= (Segment == null && other.Segment == null)
                        || (Segment != null && Segment.Equals(other.Segment));

            return result;
        }

        #endregion
    }
}