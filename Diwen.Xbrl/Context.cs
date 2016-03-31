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
    using System.Diagnostics;
    using System.Xml.Serialization;

    [DebuggerDisplay("{Id}")]
    [Serializable]
    [XmlRoot(ElementName = "context", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Context : IEquatable<Context>
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
            if(Scenario != null)
            {
                result = (Scenario.ExplicitMembers != null && Scenario.ExplicitMembers.Count != 0)
                || (Scenario.TypedMembers != null && Scenario.TypedMembers.Count != 0);
            }
            return result;
        }

        public Context()
        {
        }

        public Context(Scenario scenario)
        {
            Scenario = scenario;
        }

        public ExplicitMember AddExplicitMember(string dimension, string value)
        {
            return Scenario.ExplicitMembers.Add(dimension, value);
        }

        public TypedMember AddTypedMember(string dimension, string domain, string value)
        {
            return Scenario.TypedMembers.Add(dimension, domain, value);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Context;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            var result = 0;
            if(Entity != null)
            {
                result ^= Entity.GetHashCode();
            }

            if(Period != null)
            {
                result ^= Period.GetHashCode();
            }

            if(Scenario != null)
            {
                result ^= Scenario.GetHashCode();
            }
            return result;
        }

        #region IEquatable implementation

        public bool Equals(Context other)
        {
            var result = false;
            if(other != null)
            {
                if((Entity == null && other.Entity == null) || Entity.Equals(other.Entity))
                {
                    if((Period == null && other.Period == null) || Period.Equals(other.Period))
                    {
                        result |= (Scenario == null && other.Scenario == null) || (Scenario != null && Scenario.Equals(other.Scenario));
                    }
                }
            }
            return result;
        }

        #endregion
    }
}