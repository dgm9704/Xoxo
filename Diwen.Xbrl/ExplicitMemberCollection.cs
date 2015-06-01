namespace Diwen.Xbrl
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;

    public class ExplicitMemberCollection : Collection<ExplicitMember>, IEquatable<ExplicitMemberCollection>
    {
        private Xbrl instance;

        [XmlIgnore]
        public Xbrl Instance
        {
            get { return instance; }
            set
            {
                instance = value;
                foreach (var item in Items)
                {
                    if (item.Dimension.Namespace != Instance.DimensionNamespace)
                    {
                        item.Dimension = new XmlQualifiedName(item.Dimension.Name, Instance.DimensionNamespace);
                    }

                    if (string.IsNullOrEmpty(item.Value.Namespace))
                    {
                        string val = item.Value.Name;
                        string valPrefix = val.Substring(0, val.IndexOf(':'));
                        string valNs = this.Instance.Namespaces.LookupNamespace(valPrefix);

                        if (!string.IsNullOrEmpty(valNs))
                        {
                            if (item.Value.Namespace != valNs)
                            {
                                val = val.Substring(val.IndexOf(':') + 1);
                                item.Value = new XmlQualifiedName(val, valNs);
                            }
                        }
                        else if (this.Instance.CheckExplicitMemberDomainExists)
                        {
                            throw new InvalidOperationException(string.Format("No namespace declared for domain '{0}'", valPrefix));
                        }
                    }
                }
            }
        }

        public ExplicitMemberCollection()
        {
        }

        public ExplicitMemberCollection(Xbrl instance)
            : this()
        {
            this.Instance = instance;
        }

        public ExplicitMember Add(string dimension, string value)
        {
            XmlQualifiedName dim;
            XmlQualifiedName val;

            if (this.Instance != null)
            {
                string dimNs = this.Instance.DimensionNamespace;
                string valPrefix = value.Substring(0, value.IndexOf(':'));
                string valNs = this.Instance.Namespaces.LookupNamespace(valPrefix);
                if (this.Instance.CheckExplicitMemberDomainExists)
                {
                    if (string.IsNullOrEmpty(valNs))
                    {
                        throw new InvalidOperationException(string.Format("No namespace declared for domain '{0}'", valPrefix));
                    }
                }

                dim = new XmlQualifiedName(dimension, dimNs);
                val = new XmlQualifiedName(value, valNs);
            }
            else
            {
                dim = new XmlQualifiedName(dimension);
                val = new XmlQualifiedName(value);
            }

            var explicitMember = new ExplicitMember(dim, val);
            base.Add(explicitMember);
            return explicitMember;
        }


        #region IEquatable implementation

        public bool Equals(ExplicitMemberCollection other)
        {
            return this.SequenceEqual(other);
        }

        #endregion
    }
}