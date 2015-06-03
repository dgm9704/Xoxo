namespace Diwen.Xbrl
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "period", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Period : IEquatable<Period>
    {
        [XmlElement(ElementName = "instant", DataType = "date", Namespace = "http://www.xbrl.org/2003/instance")]
        public DateTime Instant { get; set; }

        public Period()
        {

        }

        public Period(DateTime instant)
            : this()
        {
            this.Instant = instant;
        }

        public Period(int year, int month, int day)
            : this()
        {
            this.Instant = new DateTime(year, month, day);
        }

        #region IEquatable implementation

        public bool Equals(Period other)
        {
            return other != null && this.Instant.Equals(other.Instant);
        }

        #endregion
    }
}