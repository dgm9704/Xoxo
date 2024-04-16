//
//  This file is part of Diwen.xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2024 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.Xbrl.Xml
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

        Context contextField;

        [XmlIgnore]
        public Context Context
        {
            get { return contextField; }
            set
            {
                contextField = value;
                ContextRef = contextField.Id;
            }
        }

        public FilingIndicator()
        {
            Filed = true;
        }

        public FilingIndicator(Context context, string value)
            : this(context, value, true)
        {
        }

        public FilingIndicator(Context context, string value, bool filed)
            : this()
        {
            ArgumentNullException.ThrowIfNull(context);

            Context = context;
            Value = value;
            Filed = filed;
        }

        public override bool Equals(object obj)
        => Equals(obj as FilingIndicator);

        public override int GetHashCode()
        => Value.GetHashCode();

        public override string ToString()
        => $"Value={Value}, Filed={Filed}, Context={ContextRef}";

        #region IEquatable implementation

        public bool Equals(FilingIndicator other)
        => other != null
            && Filed == other.Filed
            && Value.Equals(other.Value, StringComparison.Ordinal);

        #endregion
    }
}