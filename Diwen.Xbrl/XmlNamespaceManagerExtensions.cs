namespace Diwen.Xbrl
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.Serialization;

	public static class XmlNamespaceManagerExtensions
	{
		public static XmlSerializerNamespaces ToXmlSerializerNamespaces(this Instance instance)
		{
			if(instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			List<string> usedDomains = instance.GetUsedDomainNamespaces();

			var result = new XmlSerializerNamespaces();
			foreach(var item in Instance.DefaultNamespaces)
			{
				result.Add(item.Key, item.Value);
			}

			var foo = new List<string>{ instance.FactNamespace, instance.DimensionNamespace, instance.TypedDomainNamespace };

			foreach(var item in foo)
			{
				result.Add(instance.Namespaces.LookupPrefix(item), item);
			}

			foreach(var item in usedDomains)
			{
				result.Add(instance.Namespaces.LookupPrefix(item), item);
			}
			return result;
		}
	}
}