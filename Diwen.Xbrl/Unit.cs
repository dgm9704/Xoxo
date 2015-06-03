namespace Diwen.Xbrl
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "unit", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Unit : IEquatable<Unit>
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("measure")]
        public string Measure { get; set; }

        public Unit()
        {

        }

        public Unit(string id, string measure)
            : this()
        {
            this.Id = id;
            this.Measure = measure;
        }

        #region IEquatable implementation

        public bool Equals(Unit other)
        {
            return other != null
            && this.Id.Equals(other.Id)
            && this.Measure.Equals(other.Measure);
        }

        #endregion
    }

}