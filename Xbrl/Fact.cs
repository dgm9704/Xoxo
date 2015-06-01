namespace Xoxo
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public class Fact : IEquatable<Fact>
    {
        private static XmlDocument doc = new XmlDocument();

        [XmlIgnore]
        public string Unit { get; set; }

        [XmlIgnore]
        public string Decimals { get; set; }

        [XmlIgnore]
        public string Context{ get; set; }

        [XmlIgnore]
        public XmlQualifiedName Metric { get; set; }

        [XmlIgnore]
        public string Value { get; set; }

        [XmlIgnore]
        public string NamespaceUri { get; set; }

        public Fact()
        {
        }

        public Fact(Context context, string metric, string unit, string decimals, string value, string namespaceUri, string prefix)
        {
            this.Metric = new XmlQualifiedName(prefix + ":" + metric, namespaceUri);
            this.Unit = unit;
            this.Decimals = decimals;
            this.Context = context.Id;
            this.Value = value;
        }

        internal XmlElement ToXmlElement()
        {
            var element = doc.CreateElement(this.Metric.Name, this.Metric.Namespace);
            element.SetAttribute("unitRef", this.Unit);
            element.SetAttribute("decimals", this.Decimals);
            element.SetAttribute("contextRef", this.Context);
            element.InnerText = this.Value;
            return element;
        }

        internal static Fact FromXmlElement(XmlElement element)
        {
            var fact = new Fact();
            fact.Metric = new XmlQualifiedName(element.Name, element.NamespaceURI);
            fact.Unit = element.GetAttribute("unitRef");
            fact.Decimals = element.GetAttribute("decimals");
            fact.Context = element.GetAttribute("contextRef");
            fact.Value = element.InnerText;
            return fact;
        }

        #region IEquatable implementation

        public bool Equals(Fact other)
        {
            return this.Metric.Equals(other.Metric)
            && this.Value.Equals(other.Value)
            && this.Decimals.Equals(other.Decimals)
            && this.Unit.Equals(other.Unit);
        }

        #endregion
    }
}