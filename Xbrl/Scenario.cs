namespace Xoxo
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "scenario", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Scenario : IEquatable<Scenario>
    {
        private Xbrl instance;

        [XmlIgnore]
        public Xbrl Instance
        {
            get { return instance; }
            set
            {
                this.instance = value;
                this.ExplicitMembers.Instance = value;
                this.TypedMembers.Instance = value;
            }
        }

        [XmlElement("explicitMember", Namespace = "http://xbrl.org/2006/xbrldi")]
        public ExplicitMemberCollection ExplicitMembers { get; set; }

        [XmlElement("typedMember", Namespace = "http://xbrl.org/2006/xbrldi")]
        public TypedMemberCollection TypedMembers { get; set; }

        public Scenario()
        {
            this.ExplicitMembers = new ExplicitMemberCollection();
            this.TypedMembers = new TypedMemberCollection();
        }

        public Scenario(Xbrl instance)
        {
            this.ExplicitMembers = new ExplicitMemberCollection(instance);
            this.TypedMembers = new TypedMemberCollection(instance);
        }

        #region IEquatable implementation

        public bool Equals(Scenario other)
        {
            return other != null
            && this.ExplicitMembers.Equals(other.ExplicitMembers)
            && this.TypedMembers.Equals(other.TypedMembers);
        }

        #endregion

        public ExplicitMember AddExplicitMember(string dimension, string value)
        {
            return this.ExplicitMembers.Add(dimension, value);
        }

        public TypedMember AddTypedMember(string dimension, string domain, string value)
        {
            return this.TypedMembers.Add(dimension, domain, value);
        }

    }
}