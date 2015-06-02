namespace Diwen.Xbrl
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public static class XmlNamespaceManagerExtensions
    {
        public static XmlSerializerNamespaces ToXmlSerializerNamespaces(this XmlNamespaceManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            var result = new XmlSerializerNamespaces();
            foreach (var ns in manager.GetNamespacesInScope(XmlNamespaceScope.All))
            {
                result.Add(ns.Key, ns.Value);
            }
            return result;
        }
    }
}

