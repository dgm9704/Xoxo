namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;

    public class ExplicitMemberCollection : SortedSet<ExplicitMember>, IEquatable<ExplicitMemberCollection>
    {
        private Instance instance;
        private IFormatProvider ic = CultureInfo.InvariantCulture;

        [XmlIgnore]
        public Instance Instance
        {
            get { return instance; }
            set
            {
                instance = value;
                foreach (var item in this)
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
                            throw new InvalidOperationException(string.Format(ic, "No namespace declared for domain '{0}'", valPrefix));
                        }
                    }
                }
            }
        }

        public ExplicitMemberCollection()
        {
        }

        public ExplicitMemberCollection(Instance instance)
            : this()
        {
            this.Instance = instance;
        }


        public ExplicitMember Add(string dimension, string value)
        {
            if (string.IsNullOrEmpty(dimension))
            {
                throw new ArgumentOutOfRangeException("dimension");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentOutOfRangeException("value");
            }

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
                        throw new InvalidOperationException(string.Format(ic, "No namespace declared for domain '{0}'", valPrefix));
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

        public object this[int idx]
        {
            get { return null; }
            set { ; }
        }

        public void Add(object obj)
        {
            if (obj != null)
            {
                var nodes = obj as XmlNode[];
                {
                    var dimension = nodes[0].Value;
                    var value = nodes[1].Value;
                    this.Add(dimension, value);
                }
            }
        }
    }
}