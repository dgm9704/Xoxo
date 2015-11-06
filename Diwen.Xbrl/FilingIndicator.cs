namespace Diwen.Xbrl
{
    using System;
    using System.Diagnostics;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [DebuggerDisplay("{Value} : {Filed}")]
    [Serializable]
    [XmlRoot("filingIndicator", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
    public class FilingIndicator : IEquatable<FilingIndicator>
    {
        [XmlAttribute("contextRef")]
        public string ContextRef { get; set; }

        [XmlAttribute(AttributeName = "filed", Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
        public bool Filed { get; set; }

        [XmlText]
        public string Value { get; set; }

        private Context context;
        [XmlIgnore]
        public Context Context
        {
            get { return context; }
            set { context = value; ContextRef = context.Id; }
        }

        public FilingIndicator()
        {
            this.Filed = true;
        }

        public FilingIndicator(Context context, string value)
            : this(context, value, true)
        {
        }

        public FilingIndicator(Context context, string value, bool filed)
            : this()
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.Context = context;
            this.Value = value;
            this.Filed = filed;
        }

        public override bool Equals(object obj)
        {
            var other = obj as FilingIndicator;
            if (other != null)
            {
                return this.Equals(other);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode() ^ this.Filed.GetHashCode();
        }

        #region IEquatable implementation

        public bool Equals(FilingIndicator other)
        {
            return other != null
            && this.Filed == other.Filed
            && this.Value.Equals(other.Value, StringComparison.Ordinal);
        }

        #endregion
    }
}