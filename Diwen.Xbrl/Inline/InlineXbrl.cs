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

namespace Diwen.Xbrl.Inline
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Diwen.Xbrl.Xml;

    /// <summary/>
    public static class InlineXbrl
    {
        private static readonly IFormatProvider ic = CultureInfo.InvariantCulture;

        /// <summary/>
        public static Report ParseReportFiles(params ReportFile[] reportFiles)
        => ParseXDocuments(
            reportFiles.
                Select(f => f.Content as XDocument).
                Where(r => r != null).
                ToArray());

        /// <summary/>
        public static Report ParseFiles(params string[] files)
        => ParseXDocuments(
            files.
                Select(file => XDocument.Load(file)).
                ToArray());

        /// <summary/>
        public static Report ParseXDocuments(params XDocument[] documents)
        {
            var report = new Report();

            foreach (var document in documents)
            {
                ParseNamespaces(document, report);

                ParseSchemaReference(document, report);

                ParseContexts(document, report);

                ParseUnits(document, report);

                ParseFacts(document, report);
            }

            foreach (var item in report.Namespaces.GetNamespacesInScope(XmlNamespaceScope.All))
                report.XmlSerializerNamespaces.Add(item.Key, item.Value);

            Report.CleanupAfterDeserialization(report, ReportOptions.None);

            return report;
        }

        /// <summary/>
        public static void ParseFacts(XDocument document, Report report)
        {
            var factElements = FindFacts(document);
            var facts = new FactCollection(report);
            foreach (var factElement in factElements)
            {
                var name = factElement.Attribute("name").Value;
                var prefix = name.Split(':').First();
                var ns = document.Root.GetNamespaceOfPrefix(prefix).ToString();
                var unitRef = factElement.Attribute("unitRef")?.Value;
                var decimals = factElement.Attribute("decimals")?.Value;
                var value = factElement.Value;
                var scale = factElement.Attribute("scale")?.Value;

                if (!string.IsNullOrWhiteSpace(scale))
                {
                    var power = int.Parse(scale);
                    var v = decimal.Parse(value.Replace(" ", ""), CultureInfo.InvariantCulture);
                    var multiplier = (decimal)Math.Pow(10, power);
                    v *= multiplier;
                    value = v.ToString();
                }

                var contextRef = factElement.Attribute("contextRef").Value;
                var fact = new Fact(name, ns, unitRef, decimals, contextRef, value);
                facts.Add(fact);
            }
            foreach (var fact in facts)
                report.Facts.Add(fact);
        }

        /// <summary/>
        public static IEnumerable<XElement> FindFacts(XDocument document)
        {
            // parse facts and add to instance
            return document.Root.Descendants().Where(d => d.Attribute("contextRef") != null);
        }

        /// <summary/>
        public static void ParseUnits(XDocument document, Report report)
        {
            // parse units and add to instance
            // ix:header/ix:resources/xbrli:unit
            var unitNs = document.Root.GetNamespaceOfPrefix("xbrli");
            var unitElements = document.Root.Descendants(unitNs + "unit");
            var unitSerializer = new XmlSerializer(typeof(Unit));
            var units = new UnitCollection(report);
            foreach (var unitElement in unitElements)
            {
                var unitReader = unitElement.CreateReader();
                var unit = (Unit)unitSerializer.Deserialize(unitReader);
                units.Add(unit);
            }
            report.Units = units;
        }

        /// <summary/>
        public static void ParseContexts(XDocument document, Report report)
        {
            // parse contexts and add to instance
            // ix:header/ix:resources/xbrli:context
            var contextNs = document.Root.GetNamespaceOfPrefix("xbrli");
            var contextElements = document.Root.Descendants(contextNs + "context");
            var contextSerializer = new XmlSerializer(typeof(Context));

            var contexts = new ContextCollection(report);
            foreach (var contextElement in contextElements)
            {
                var contextReader = contextElement.CreateReader();
                var context = (Context)contextSerializer.Deserialize(contextReader);
                contexts.Add(context);
            }
            report.Contexts = contexts;
        }

        /// <summary/>
        public static void ParseSchemaReference(XDocument document, Report report)
        {
            // parse schemaRefs and add to instance
            // ix:header/ix:references/link:schemaRef

            var linkNs = document.Root.GetNamespaceOfPrefix("link");
            var schemaRefElements = document.Root.Descendants(linkNs + "schemaRef");

            var schemaRefSerializer = new XmlSerializer(typeof(SchemaReference));
            foreach (var schemaRefElement in schemaRefElements)
            {
                using (var schemaReader = schemaRefElement.CreateReader())
                {
                    var schemaRef = (SchemaReference)schemaRefSerializer.Deserialize(schemaReader);
                    report.SchemaReferences.Add(schemaRef);
                }
            }
        }

        /// <summary/>
        public static void ParseNamespaces(XDocument document, Report report)
        {
            // parse namespaces from xhtml and add to instance
            var namespaces =
                document.
                Root.
                Attributes().
                Where(a => a.IsNamespaceDeclaration).
                Where(a => a.Name.LocalName != "xmlns").
                ToDictionary(
                    g => g.Name.LocalName,
                    g => g.Value);

            foreach (var ns in namespaces)
                report.Namespaces.AddNamespace(ns.Key, ns.Value);
        }
    }
}