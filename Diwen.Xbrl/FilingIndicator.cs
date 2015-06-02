namespace Diwen.Xbrl
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot("filingIndicator", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
    public class FilingIndicator : IEquatable<FilingIndicator>
    {
        [XmlAttribute("contextRef")]
        public string ContextId { get; set; }

        [XmlText]
        public string Value { get; set; }

        public FilingIndicator()
        {
        }

        public FilingIndicator(Context context, string value)
            : this()
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.ContextId = context.Id;
            this.Value = value;
        }

        #region IEquatable implementation

        public bool Equals(FilingIndicator other)
        {
            return other != null && this.Value.Equals(other.Value);
        }

        #endregion
    }

}