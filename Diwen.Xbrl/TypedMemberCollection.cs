namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;

    public class TypedMemberCollection : SortedSet<TypedMember>, IEquatable<TypedMemberCollection>
    {

        private Instance instance;

        [XmlIgnore]
        public Instance Instance
        {
            get { return instance; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                instance = value;
                var dimNs = instance.DimensionNamespace;
                var dimPrefix = instance.Namespaces.LookupPrefix(dimNs);
                var domNs = instance.TypedDomainNamespace;
                var domprefix = instance.Namespaces.LookupPrefix(domNs);

                foreach (var item in this)
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

        public bool Equals(TypedMemberCollection other)
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
                    var domain = nodes[1].Name;
                    var value = nodes[1].InnerText;
                    this.Add(dimension, domain, value);
                }
            }
        }

    }
}