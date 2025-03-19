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
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using Diwen.Xbrl.Extensions;

    /// <summary/>
    public class Fact : IEquatable<Fact>
    {
        static readonly XmlDocument doc = new();

        /// <summary/>
        [XmlIgnore]
        public Unit Unit { get; set; }

        /// <summary/>
        [XmlIgnore]
        public string Decimals { get; set; }

        /// <summary/>
        [XmlIgnore]
        public Context Context { get; set; }

        /// <summary/>
        [XmlIgnore]
        public XmlQualifiedName Metric { get; set; }

        /// <summary/>
        [XmlIgnore]
        public string Value { get; set; }

        /// <summary/>
        [XmlIgnore]
        public FactCollection Facts { get; private set; }

        /// <summary/>
        [XmlAnyElement]
        public XmlElement[] FactItems
        {
            get
            {
                return Facts.Select(f => f.ToXmlElement()).ToArray();
            }
            set
            {
                if (value != null)
                    Facts.AddRange(value.Select(e => FromXmlElement(e)));
            }
        }

        internal string ContextRef;
        internal string UnitRef;

        /// <summary/>
        public Fact()
        {
            Facts = new FactCollection(null);
        }

        /// <summary/>
        public Fact(string name, string namespaceURI, string unitRef, string decimals, string contextRef, string value) : this()
        {
            Metric = new XmlQualifiedName(name, namespaceURI);
            UnitRef = unitRef ?? "";
            Decimals = decimals ?? "";
            ContextRef = contextRef ?? "";
            Value = value ?? "";
        }

        /// <summary/>
        public Fact(Context context, string metric, Unit unit, string decimals, string value, string namespaceUri, string prefix)
            : this(context, metric, unit, decimals, value, new Uri(namespaceUri), prefix)
        {
        }

        /// <summary/>
        public Fact(Context context, string metric, Unit unit, string decimals, string value, Uri namespaceUri, string prefix) : this()
        {
            ArgumentNullException.ThrowIfNull(context);

            ArgumentNullException.ThrowIfNull(namespaceUri);

            Metric = new XmlQualifiedName($"{prefix}:{metric}", namespaceUri.ToString());
            Unit = unit;
            Decimals = decimals;
            Context = context;
            Value = value;
        }

        /// <summary/>
        public Fact AddFact(Context context, string metric, string unitRef, string decimals, string value)
        => Facts.Add(context, metric, unitRef, decimals, value);

        /// <summary/>
        public Fact AddFact(Scenario scenario, string metric, string unitRef, string decimals, string value)
        {
            if (scenario != null)
            {
                Facts.Report = scenario.Report;

                if (!scenario.ExplicitMembers.Any() && !scenario.TypedMembers.Any())
                    scenario = null;
            }
            return Facts.Add(scenario, metric, unitRef, decimals, value);
        }

        /// <summary/>
        public Fact AddFact(Segment segment, string metric, string unitRef, string decimals, string value)
        {
            if (segment != null)
            {
                Facts.Report = segment.Report;

                if (!segment.HasMembers)
                    segment = null;
            }

            return Facts.Add(segment, metric, unitRef, decimals, value);
        }

        /// <summary/>
        public override string ToString()
        {
            var metric = Metric != null ? Metric.LocalName() : string.Empty;
            var unit = Unit != null ? Unit.ToString() : string.Empty;
            return $"Metric={metric}, Value={Value}, Unit={unit}, Decimals={Decimals}, Context={ContextRef}";
        }

        internal XmlElement ToXmlElement()
        {
            var element = doc.CreateElement(Metric.Name, Metric.Namespace);

            if (Facts.Any())
            {
                element.AppendChildren(Facts.Select(f => f.ToXmlElement()));
            }
            else
            {
                if (Context != null)
                    element.SetAttribute("contextRef", Context.Id);

                if (Unit != null)
                    element.SetAttribute("unitRef", Unit.Id);

                if (!string.IsNullOrEmpty(Decimals))
                    element.SetAttribute("decimals", Decimals);

                element.InnerText = Value;
            }
            return element;
        }

        internal static Fact FromXmlElement(XmlElement element)
        {

            var unitRef = string.Empty;
            var decimals = string.Empty;
            var contextRef = string.Empty;
            var value = string.Empty;

            if (!element.ChildNodes.OfType<XmlElement>().Any())
            {
                unitRef = element.GetAttribute("unitRef");
                decimals = element.GetAttribute("decimals");
                contextRef = element.GetAttribute("contextRef");
                value = element.InnerText;
            }

            return new Fact(element.Name, element.NamespaceURI, unitRef, decimals, contextRef, value);
        }

        /// <summary/>
        public override bool Equals(object obj)
        {
            var result = false;
            if (obj is Fact other && Equals(other))
                result |= Facts.Equals(other.Facts);

            return result;
        }

        /// <summary/>
        public override int GetHashCode()
        => Value.GetHashCode();

        #region IEquatable implementation

        /// <summary/>
        public bool Equals(Fact other)
        => other != null
            && (Value == null ? other.Value == null : Value.Equals(other.Value, StringComparison.Ordinal))
            && (Metric == null ? other.Metric == null : Metric.Equals(other.Metric))
            && (Decimals == null ? other.Decimals == null : Decimals.Equals(other.Decimals, StringComparison.Ordinal))
            && (Unit == null ? other.Unit == null : Unit.Equals(other.Unit));

        #endregion
    }
}