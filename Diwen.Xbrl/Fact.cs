//
//  This file is part of Diwen.xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2016 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.Xbrl
{
    using System;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Serialization;

    public class Fact : IEquatable<Fact>
    {
        static IFormatProvider ic = CultureInfo.InvariantCulture;

        static XmlDocument doc = new XmlDocument();

        [XmlIgnore]
        public Unit Unit { get; set; }

        [XmlIgnore]
        public string Decimals { get; set; }

        [XmlIgnore]
        public Context Context { get; set; }

        [XmlIgnore]
        public XmlQualifiedName Metric { get; set; }

        [XmlIgnore]
        public string Value { get; set; }

        internal string ContextRef;
        internal string UnitRef;

        public Fact()
        {
        }

        public override string ToString()
        {
            var metric = Metric != null ? Metric.LocalName() : string.Empty;
            var measure = Unit != null ? Unit.Measure : string.Empty;
            var scenario = string.Empty;
            if(Context != null && Context.Scenario != null)
            {
                scenario = Context.Scenario.ToString();
            }

            return string.Format(ic, "Metric={0}, Value={1}, Unit={2}, Decimals={3}, Context={4}: {5}", 
                metric, Value, measure, Decimals, ContextRef, scenario);
        }

        public Fact(Context context, string metric, Unit unit, string decimals, string value, string namespaceUri, string prefix)
            : this(context, metric, unit, decimals, value, new Uri(namespaceUri), prefix)
        {
        }

        public Fact(Context context, string metric, Unit unit, string decimals, string value, Uri namespaceUri, string prefix)
        {
            if(context == null)
            {
                throw new ArgumentNullException("context");
            }

            if(namespaceUri == null)
            {
                throw new ArgumentNullException("namespaceUri");
            }

            Metric = new XmlQualifiedName(prefix + ":" + metric, namespaceUri.ToString());
            Unit = unit;
            Decimals = decimals;
            Context = context;
            Value = value;
        }

        internal XmlElement ToXmlElement()
        {
            var element = doc.CreateElement(Metric.Name, Metric.Namespace);

            if(Context != null)
            {
                element.SetAttribute("contextRef", Context.Id);
            }

            if(Unit != null)
            {
                element.SetAttribute("unitRef", Unit.Id);
            }
            if(!string.IsNullOrEmpty(Decimals))
            {
                element.SetAttribute("decimals", Decimals);
            }

            element.InnerText = Value;
            return element;
        }

        internal static Fact FromXmlElement(XmlElement element)
        {
            var fact = new Fact();
            fact.Metric = new XmlQualifiedName(element.Name, element.NamespaceURI);

            fact.UnitRef = element.GetAttribute("unitRef");
            fact.Decimals = element.GetAttribute("decimals");
            fact.ContextRef = element.GetAttribute("contextRef");
            fact.Value = element.InnerText;
            return fact;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Fact;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Metric.GetHashCode();
        }

        #region IEquatable implementation

        public bool Equals(Fact other)
        {
            var result = other != null
                         && Metric.Equals(other.Metric)
                         && Value.Equals(other.Value, StringComparison.Ordinal)
                         && Decimals.Equals(other.Decimals, StringComparison.Ordinal);
            if(result)
            {
                result = (Unit == null && other.Unit == null)
                || Unit.Equals(other.Unit);
            }

            return result;
        }

        #endregion
    }
}