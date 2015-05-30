namespace Xoxo
{
	using System.Xml.Serialization;
	using System.Xml;

	public static class XmlNamespaceManagerExtensions
	{
		public static XmlSerializerNamespaces ToXmlSerializerNamespaces(this XmlNamespaceManager manager)
		{
			var result = new XmlSerializerNamespaces();
			foreach(var ns in manager.GetNamespacesInScope(XmlNamespaceScope.All))
			{
				result.Add(ns.Key, ns.Value);
			}
			return result;
		}
	}
}

