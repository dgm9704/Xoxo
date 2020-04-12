//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2020 John Nordberg
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

namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public static class InlineXbrl
    {
        private static IFormatProvider ic = CultureInfo.InvariantCulture;

        private static List<Func<XDocument, string>> EsefValidations = new List<Func<XDocument, string>>
        {
            G2_1_2,
            G2_1_3_1,
            G2_1_3_2,
            G2_2_1,
            G2_2_2,
        };

        private static string G2_1_2(XDocument report)
        {
            var xbrli = report.Root.GetNamespaceOfPrefix("xbrli");
            var periodElements = report.Root.Descendants(xbrli + "period");
            var dateElements = periodElements.SelectMany(p => p.Descendants());

            return dateElements.
                Any(p => !DateTime.TryParseExact(p.Value, "yyyy-MM-dd", ic, DateTimeStyles.None, out DateTime value))
                    ? "<xbrli:period> element should contain values in YYYY-MM-DD format without time component"
                    : null;
        }

        private static string G2_1_3_1(XDocument report)
        {
            var xbrli = report.Root.GetNamespaceOfPrefix("xbrli");
            var segmentElements = report.Root.Descendants(xbrli + "segment");

            return segmentElements.Any()
                    ? "<xbrli:scenario> element must be used instead of <xbrli:segment>"
                    : null;
        }

        private static string G2_1_3_2(XDocument report)
        {
            var xbrli = report.Root.GetNamespaceOfPrefix("xbrli");
            var xbrldi = report.Root.GetNamespaceOfPrefix("xbrldi");
            var scenarioElements = report.Root.Descendants(xbrli + "scenario");
            var customElements = scenarioElements.SelectMany(s => s.Descendants().Where(e => e.Name.Namespace != xbrldi));

            return customElements.Any()
                    ? "<xbrli:scenario> in contexts MUST NOT contain any other content than defined in XBRL Dimensions specification"
                    : null;
        }

        private static string G2_2_1(XDocument report)
        {
            var factElements = FindFacts(report);
            return factElements.Any(e => e.Attribute("precision") != null)
                    ? "The accuracy of numeric facts SHOULD be defined with the 'decimals' attribute rather than the 'precision' attribute"
                    : null;
        }

        private static string G2_2_2(XDocument report)
        {
            // var num = report.Root.GetNamespaceOfPrefix("num");
            // var percents = report.Root.Descendants(num + "percentItemType");
            // return percents.Any(e => Decimal.Parse(e.Value) > 1)
            //         ? "All elements of num:percentItemType should be less or equal to 1"
            //         : null;

            // checking this requires loading documents outside the report
            return null;
        }



        public static ValidationResult ValidateEsef(string path)
        => ValidateEsef(XDocument.Load(path));

        public static ValidationResult ValidateEsef(XDocument report)
        {
            var messages =
                EsefValidations.
                Select(validation => validation(report)).
                Where(message => !string.IsNullOrWhiteSpace(message));

            return new ValidationResult(!messages.Any(), messages.ToArray());
        }

        public static Instance ParseInstance(XDocument report)
        {
            var instance = new Instance();

            ParseNamespaces(report, instance);

            ParseSchemaReference(report, instance);

            ParseContexts(report, instance);

            ParseUnits(report, instance);

            ParseFacts(report, instance);

            return instance;
        }

        // public static ValidationResult ValidateEsef(string inputFile)
        //  FormatValidations.GetValueOrDefault(InlineXbrlType.Esef, (f) => throw new ArgumentOutOfRangeException(nameof(InlineXbrlType.Esef)))(inputFile);

        private static void ParseFacts(XDocument report, Instance instance)
        {
            var factElements = FindFacts(report);
            var facts = new FactCollection(instance);
            foreach (var factElement in factElements)
            {
                var name = factElement.Attribute("name").Value;
                var prefix = name.Split(':').First();
                var ns = report.Root.GetNamespaceOfPrefix(prefix).ToString();
                var unitRef = factElement.Attribute("unitRef")?.Value;
                var decimals = factElement.Attribute("decimals")?.Value;
                var value = factElement.Value;
                var scale = factElement.Attribute("scale")?.Value;

                if (!string.IsNullOrWhiteSpace(scale))
                {
                    var power = int.Parse(scale);
                    var v = decimal.Parse(value.Replace(" ", ""));
                    var multiplier = (decimal)Math.Pow(10, power);
                    v *= multiplier;
                    value = v.ToString();
                }

                var contextRef = factElement.Attribute("contextRef").Value;
                var fact = new Fact(name, ns, unitRef, decimals, contextRef, value);
                facts.Add(fact);
            }
            foreach (var fact in facts)
                instance.Facts.Add(fact);
        }

        private static IEnumerable<XElement> FindFacts(XDocument report)
        {
            // parse facts and add to instance
            return report.Descendants().Where(d => d.Attribute("contextRef") != null);
        }

        private static void ParseUnits(XDocument report, Instance instance)
        {
            // parse units and add to instance
            // ix:header/ix:resources/xbrli:unit
            var unitNs = report.Root.GetNamespaceOfPrefix("xbrli");
            var unitElements = report.Root.Descendants(unitNs + "unit");
            var unitSerializer = new XmlSerializer(typeof(Unit));
            var units = new UnitCollection(instance);
            foreach (var unitElement in unitElements)
            {
                var unitReader = unitElement.CreateReader();
                var unit = (Unit)unitSerializer.Deserialize(unitReader);
                units.Add(unit);
            }
            instance.Units = units;
        }

        private static void ParseContexts(XDocument report, Instance instance)
        {
            // parse contexts and add to instance
            // ix:header/ix:resources/xbrli:context
            var contextNs = report.Root.GetNamespaceOfPrefix("xbrli");
            var contextElements = report.Root.Descendants(contextNs + "context");
            var contextSerializer = new XmlSerializer(typeof(Context));

            var contexts = new ContextCollection(instance);
            foreach (var contextElement in contextElements)
            {
                var contextReader = contextElement.CreateReader();
                var context = (Context)contextSerializer.Deserialize(contextReader);
                contexts.Add(context);
            }
            instance.Contexts = contexts;
        }

        private static void ParseSchemaReference(XDocument report, Instance instance)
        {
            // parse schemaRef and add to instance
            // ix:header/ix:references/link:schemaRef
            var link = report.Root.GetNamespaceOfPrefix("link");
            // there can be only one
            var schemaRefElements = report.Root.Descendants(link + "schemaRef");
            var schemaRefSerializer = new XmlSerializer(typeof(SchemaReference));
            var schemaRefElement = schemaRefElements.Single();
            var schemaReader = schemaRefElement.CreateReader();
            var schemaRef = (SchemaReference)schemaRefSerializer.Deserialize(schemaReader);
            instance.SchemaReference = schemaRef;
        }

        private static void ParseNamespaces(XDocument report, Instance instance)
        {
            // parse namespaces from xhtml and add to instance
            var namespaces =
                report.
                Root.
                Attributes().
                Where(a => a.IsNamespaceDeclaration).
                Where(a => a.Name.LocalName != "xmlns").
                ToDictionary(
                    g => g.Name.LocalName,
                    g => g.Value);

            foreach (var ns in namespaces)
                instance.Namespaces.AddNamespace(ns.Key, ns.Value);
        }

        public static Instance ParseInstance(string path)
        => ParseInstance(XDocument.Load(path));
    }
}