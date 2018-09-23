//
//  This file is part of Diwen.xbrl.
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
    using System.Diagnostics;
    using System.Linq;
    using System.Xml.Serialization;

    [DebuggerDisplay("{Id}")]
    [Serializable]
    [XmlRoot(ElementName = "context", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Context : IEquatable<Context>, IXbrlObject
    {
        [XmlAttribute("id", Namespace = "http://www.xbrl.org/2003/instance")]
        public string Id { get; set; }

        [XmlElement("entity", Namespace = "http://www.xbrl.org/2003/instance")]
        public Entity Entity { get; set; }

        [XmlElement("period", Namespace = "http://www.xbrl.org/2003/instance")]
        public Period Period { get; set; }

        [XmlElement("scenario", Namespace = "http://www.xbrl.org/2003/instance")]
        public Scenario Scenario { get; set; }

        public bool ShouldSerializeScenario()
        {
            var result = false;
            if (Scenario != null)
            {
                result = (Scenario.ExplicitMembers.Any())
                    || (Scenario.TypedMembers.Any());
            }
            return result;
        }

        public Context() { }

        public Context(Scenario scenario)
        => Scenario = scenario;

        public Context(Entity entity, Segment segment)
        => entity.Segment = segment;

        public ExplicitMember AddExplicitMember(string dimension, string value)
        => Scenario.ExplicitMembers.Add(dimension, value);

        public TypedMember AddTypedMember(string dimension, string domain, string value)
        => Scenario.TypedMembers.Add(dimension, domain, value);

        public override bool Equals(object obj)
        => Equals(obj as Context);

        public override int GetHashCode()
        => (Period != null ? Period.GetHashCode() : 0)
            + 7 * (Entity != null ? Entity.GetHashCode() : 0)
            + 31 * (Scenario != null ? Scenario.GetHashCode() : 0);

        #region IEquatable implementation

        public bool Equals(Context other)
        {
            var result = false;
            if (other != null)
                if ((Period == null && other.Period == null) || (Period != null && Period.Equals(other.Period)))
                    if ((Entity == null && other.Entity == null) || Entity.Equals(other.Entity))
                        result |= (Scenario == null && other.Scenario == null) || Scenario.Equals(other.Scenario);

            return result;
        }

        #endregion
    }
}