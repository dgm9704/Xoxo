namespace Xoxo
{
    using System.Collections.ObjectModel;
    using System;
    using System.Xml.Serialization;
    using System.Xml;
    using System.Linq;

    public class TypedMemberCollection : Collection<TypedMember>, IEquatable<TypedMemberCollection>
    {

        private Xbrl instance;

        [XmlIgnore]
        public Xbrl Instance
        {
            get { return instance; }
            set
            {
                instance = value;
                var dimNs = instance.DimensionNamespace;
                var dimPrefix = instance.Namespaces.LookupPrefix(dimNs);
                var domNs = instance.TypedDomainNamespace;
                var domprefix = instance.Namespaces.LookupPrefix(domNs);

                foreach (var item in Items)
                {
                    if (item.Dimension.Namespace != instance.DimensionNamespace)
                    {
                        item.Dimension = new XmlQualifiedName(dimPrefix + ":" + item.Dimension.Name, dimNs);
                    }

                    if (item.Domain.Namespace != instance.TypedDomainNamespace)
                    {
                        item.Domain = new XmlQualifiedName(domprefix + ":" + item.Domain.Name, domNs);
                    }
                }
            }
        }

        public TypedMemberCollection()
        {
			
        }

        public TypedMemberCollection(Xbrl instance)
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

        public bool Equals(TypedMemberCollection other)
        {
            return this.SequenceEqual(other);
        }

        #endregion
    }
}