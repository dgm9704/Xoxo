namespace Xoxo
{
    using System;
    using System.Xml.Serialization;

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
            return Scenario != null &&
            ((Scenario.ExplicitMembers != null && Scenario.ExplicitMembers.Count != 0)
            || (Scenario.TypedMembers != null && Scenario.TypedMembers.Count != 0));
        }

        public bool ScenarioSpecified
        {
            get
            {
                return Scenario != null &&
                ((Scenario.ExplicitMembers != null && Scenario.ExplicitMembers.Count != 0)
                || (Scenario.TypedMembers != null && Scenario.TypedMembers.Count != 0));
            }
        }

        public Context()
        {
        }

        public Context(string id)
            : this()
        {
            this.Id = id;
        }

        public Context(Scenario scenario)
        {
            this.Scenario = scenario;
        }

        #region IEquatable implementation

        public bool Equals(Context other)
        {
            var result = false;
            if ((this.Entity == null && other.Entity == null) || this.Entity.Equals(other.Entity))
            {
                if ((this.Period == null && other.Period == null) || this.Period.Equals(other.Period))
                {
                    if ((this.Scenario == null && other.Scenario == null) || this.Scenario.Equals(other.Scenario))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        #endregion

        public ExplicitMember AddExplicitMember(string dimension, string value)
        {
            return this.Scenario.ExplicitMembers.Add(dimension, value);
        }

        public TypedMember AddTypedMember(string dimension, string domain, string value)
        {
            return this.Scenario.TypedMembers.Add(dimension, domain, value);
        }
    }
}