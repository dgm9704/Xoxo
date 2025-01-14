//
//  This file is part of Diwen.Xbrl.
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
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using Diwen.Xbrl.Extensions;
#if NETSTANDARD2_0
    using ArgumentNullException = Compat.ArgumentNullException;
#endif
    
    /// <summary/>
    [DebuggerDisplay("{Id}")]
    [Serializable]
    [XmlRoot(ElementName = "unit", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Unit : IXmlSerializable, IEquatable<Unit>, IXbrlObject
    {

        internal Report Report { get; set; }

        /// <summary/>
        [XmlIgnore]
        public string Id { get; set; }

        /// <summary/>
        [XmlIgnore]
        public XmlQualifiedName Measure { get; set; }

        /// <summary/>
        public Unit() { }

        /// <summary/>
        public Unit(string id, string value)
            : this()
        {
            Id = id;
            Measure = new XmlQualifiedName(value);
        }

        /// <summary/>
        public Unit(string id, XmlQualifiedName measure)
            : this()
        {
            Id = id;
            Measure = measure;
        }

        /// <summary/>
        public override string ToString()
        => $"{Measure.Namespace}:{Measure.LocalName()}";

        /// <summary/>
        public override bool Equals(object obj)
        => Equals(obj as Unit);

        #region IEquatable implementation

        /// <summary/>
        public bool Equals(Unit other)
        => other != null
            && Measure.LocalName().Equals(other.Measure.LocalName(), StringComparison.Ordinal)
            && Measure.Namespace.Equals(other.Measure.Namespace, StringComparison.Ordinal);

        /// <summary/>
        public override int GetHashCode()
        => Measure != null ? Measure.Namespace.GetHashCode() * Measure.LocalName().GetHashCode() : 0;

        /// <summary/>
        public string ComparisonMessage()
        => ToString();

        #endregion

        #region IXmlSerializable implementation

        /// <summary/>
        public XmlSchema GetSchema()
        => null;

        /// <summary/>
        public void ReadXml(XmlReader reader)
        {
            ArgumentNullException.ThrowIfNull(reader);

            reader.MoveToContent();
            Id = reader.GetAttribute("id");
            reader.ReadStartElement();
            var prefix = reader.Prefix;

            var content = reader.ReadString().Trim();
            var idx = content.IndexOf(':');

            if (idx != -1)
                prefix = content.Substring(0, idx);

            var ns = reader.LookupNamespace(prefix);

            if (string.IsNullOrEmpty(ns))
                Console.WriteLine("Bar");

            var name = content.Substring(idx + 1);
            Measure = new XmlQualifiedName(name, ns);
            reader.ReadEndElement();
            reader.ReadEndElement();
        }

        /// <summary/>
        public void WriteXml(XmlWriter writer)
        {
            ArgumentNullException.ThrowIfNull(writer);

            writer.WriteAttributeString("id", Id);
            var prefix = Measure.Prefix();
            if (string.IsNullOrEmpty(prefix))
                prefix = Report.Namespaces.LookupPrefix(Measure.Namespace);

            var value = $"{prefix}:{Measure.LocalName()}";
            writer.WriteElementString("measure", "http://www.xbrl.org/2003/instance", value);
        }

        #endregion

    }
}