namespace Diwen.Xbrl
{
    using System;
    using System.Xml;

    public static class XmlQualifiedNameExtensions
    {
        public static string LocalName(this XmlQualifiedName value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var result = string.Empty;

            string name = value.Name;
            if (!string.IsNullOrEmpty(name))
            {
                result = name.Substring(name.IndexOf(':') + 1);
            }

            return result;
        }

        public static string Prefix(this XmlQualifiedName value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var result = string.Empty;

            string name = value.Name;
            if (!string.IsNullOrEmpty(name))
            {
                var idx = name.IndexOf(':');
                if (idx != -1)
                {
                    result = name.Substring(0, idx);
                }
            }

            return result;
        }
    }
}

