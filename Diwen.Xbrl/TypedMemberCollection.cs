namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml;
    using System.Xml.Serialization;

    public class TypedMemberCollection : Collection<TypedMember>, IEquatable<IList<TypedMember>>
    {
        private Instance instanceField;

        [XmlIgnore]
        public Instance Instance
        {
            get { return instanceField; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.instanceField = value;
                var dimNs = instanceField.DimensionNamespace;
                var dimPrefix = instanceField.Namespaces.LookupPrefix(dimNs);
                var domNs = instanceField.TypedDomainNamespace;
                var domprefix = instanceField.Namespaces.LookupPrefix(domNs);

                foreach (var item in this)
                {
                    item.Instance = value;
                    if (item.Dimension.Namespace != instanceField.DimensionNamespace)
                    {
                        item.Dimension = new XmlQualifiedName(dimPrefix + ":" + item.Dimension.Name, dimNs);
                    }

                    if (item.Domain.Namespace != instanceField.TypedDomainNamespace)
                    {
                        item.Domain = new XmlQualifiedName(domprefix + ":" + item.Domain.Name, domNs);
                    }
                }
            }
        }

        public TypedMemberCollection()
        {
        }

        public TypedMemberCollection(Instance instance)
            : this()
        {
            this.Instance = instance;
        }

        public TypedMember Add(string dimension, string domain, string value)
        {

            XmlQualifiedName dim;
            XmlQualifiedName dom;
            if (this.Instance != null)
            {
                dim = new XmlQualifiedName(dimension, this.Instance.DimensionNamespace);
                dom = new XmlQualifiedName(domain, this.Instance.TypedDomainNamespace);
            }
            else
            {
                dim = new XmlQualifiedName(dimension);
                dom = new XmlQualifiedName(domain);
            }

            var typedMember = new TypedMember(dim, dom, value);
            base.Add(typedMember);
            return typedMember;
        }

        #region IEquatable implementation

        public bool Equals(IList<TypedMember> other)
        {
            return this.ContentCompare(other);
        }

        #endregion
    }
}