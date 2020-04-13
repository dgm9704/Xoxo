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
    using Diwen.Xbrl.Extensions;

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
            G2_2_3,
            G2_3_1_1,
            G2_3_1_2,
            G2_3_1_3,
            G2_4_1_1,
            G2_4_1_2,
            G2_4_1_3,
        };


        public static EsefResult ValidateEsef(string path)
        => ValidateEsef(XDocument.Load(path));

        public static EsefResult ValidateEsef(XDocument report)
        {
            var errors =
                EsefValidations.
                Select(validation => validation(report)).
                Where(error => !string.IsNullOrWhiteSpace(error)).
                ToArray();

            return new EsefResult(errors.Any() ? "invalid" : "valid", errors.ToArray());
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

        private static string G2_1_2(XDocument report)
        {
            var errors = new HashSet<string>();
            var xbrli = report.Root.GetNamespaceOfPrefix("xbrli");
            var periodElements = report.Root.Descendants(xbrli + "period");
            var dates = periodElements.SelectMany(p => p.Descendants().Select(d => d.Value));

            if (dates.Any(d => d.IndexOf('T') != -1))
                errors.Add("periodWithTimeContent");

            var zones = new char[] { 'Z', '+', '-' };
            if (dates.Any(d => d.LastIndexOfAny(zones) > 9))
                errors.Add("periodWithTimeZone");

            return errors.Join(",");
        }

        private static string G2_1_3_1(XDocument report)
        {
            var xbrli = report.Root.GetNamespaceOfPrefix("xbrli");
            var segmentElements = report.Root.Descendants(xbrli + "segment");

            return
                segmentElements.Any()
                    ? "segmentUsed"
                    : null;
        }

        private static string G2_1_3_2(XDocument report)
        {
            var xbrli = report.Root.GetNamespaceOfPrefix("xbrli");
            var xbrldi = report.Root.GetNamespaceOfPrefix("xbrldi");
            var scenarioElements = report.Root.Descendants(xbrli + "scenario");
            var customElements = scenarioElements.SelectMany(s => s.Descendants().Where(e => e.Name.Namespace != xbrldi));

            return
                customElements.Any()
                    ? "scenarioContainsNonDimensionalContent"
                    : null;
        }

        private static string G2_2_1(XDocument report)
        {
            var factElements = FindFacts(report);
            return
                factElements.Any(e => e.Attribute("precision") != null)
                    ? "precisionAttributeUsed"
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

        private static string G2_2_3(XDocument report)
        {
            var ixt = report.Root.GetNamespaceOfPrefix("ixt");
            return !(
                ixt == null
                || ixt.NamespaceName == "http://www.xbrl.org/inlineXBRL/transformation/2015-02-26" // TR 3
                || ixt.NamespaceName == "http://www.xbrl.org/inlineXBRL/transformation/2019-04-19" // TR 4 PWD
                || ixt.NamespaceName == "http://www.xbrl.org/inlineXBRL/transformation/2020-02-12" // TR 4
            )
                ? "transformRegistry"
                : null;
        }

        private static string G2_3_1_1(XDocument report)
        {
            var ix = report.Root.GetNamespaceOfPrefix("ix");
            var footnotes = report.Root.Descendants(ix + "footnote");
            var relationships = report.Root.Descendants(ix + "relationship");

            return
                footnotes.Any(f => f.Attribute("footnoteRole") != null)
                || relationships.Any(r => r.Attribute("arcrole") != null)
                    ? "nonStandardRoleForFootnote"
                    : null;
        }

        private static string G2_3_1_2(XDocument report)
        {
            var ix = report.Root.GetNamespaceOfPrefix("ix");

            var footnotes =
                report.Root.
                    Descendants(ix + "footnote").
                    Select(f => f.Attribute("id")?.Value).
                    Where(a => !string.IsNullOrEmpty(a))
                    .ToHashSet();

            var relationships =
                report.Root.
                    Descendants(ix + "relationship").
                    Select(r => r.Attribute("toRefs")?.Value).
                    Where(a => !string.IsNullOrEmpty(a)).
                    ToHashSet();

            return
                footnotes.Except(relationships).Any()
                    ? "unusedFootnote"
                    : null;
        }

        private static string G2_3_1_3(XDocument report)
        {
            var error = new HashSet<string>();
            var ix = report.Root.GetNamespaceOfPrefix("ix");
            var xml = report.Root.GetNamespaceOfPrefix("xml");

            var reportLanguage = report.Root.Attribute(xml + "lang")?.Value ?? "";

            var footnotes =
                report.Root.
                    Descendants(ix + "footnote").
                    ToList();

            if (footnotes.Any(f => f.Attribute(xml + "lang") == null))
                error.Add("undefinedLanguageForFootnote");

            if (footnotes.
                    Select(f => f.Attribute(xml + "lang")).
                    Where(a => a != null).
                    Any(a => a.Value != reportLanguage)
            )
                error.Add("footnoteOnlyInLanguagesOtherThanLanguageOfAReport");

            return error.Join(",");
        }

        private static string G2_4_1_1(XDocument report)
        {
            var error = new HashSet<string>();
            var ix = report.Root.GetNamespaceOfPrefix("ix");

            var hiddenFacts =
                report.Root.
                Descendants(ix + "hidden").
                FirstOrDefault()?.
                Descendants().
                Where(d => d.Attribute("contextRef") != null);

            var transformableHiddenFacts =
                hiddenFacts?.
                Where(d => d.Attribute("format") != null);

            if (transformableHiddenFacts != null && transformableHiddenFacts.Any())
                error.Add("transformableElementIncludedInHiddenSection");

            var hiddenFactIds =
                hiddenFacts?.
                Select(f => f.Attribute("id").Value).
                ToHashSet() ?? new HashSet<string>();

            var reportHiddenFactIds =
                report.Root.
                    Descendants().
                    Select(d => d.Attribute("style")).
                    Where(a => a != null).
                    Select(a => a.Value.Split(':')).
                    Where(v => v.First() == "-esef-ix-hidden").
                    Select(v => v.Last()).
                    ToHashSet() ?? new HashSet<string>();

            if (reportHiddenFactIds.Except(hiddenFactIds).Any())
                error.Add("esefIxHiddenStyleNotLinkingFactInHiddenSection");

            if (hiddenFactIds.Except(reportHiddenFactIds).Any())
                error.Add("factInHiddenSectionNotInReport");

            return error.Join(",");
        }

        private static string G2_4_1_2(XDocument report)
        {
            var errors = new HashSet<string>();

            var ix = report.Root.GetNamespaceOfPrefix("ix");
            var tuples = report.Root.Descendants(ix + "tuple");

            if (tuples.Any(t => (t.Attribute("name")?.Value ?? "").IndexOf(':') != -1))
                errors.Add("tupleDefinedInExtensionTaxonomy");

            if (tuples.Any())
                errors.Add("tupleElementUsed");

            return errors.Join(",");
        }

        private static string G2_4_1_3(XDocument report)
        {
            var errors = new HashSet<string>();

            var ix = report.Root.GetNamespaceOfPrefix("ix");
            var fractions = report.Root.Descendants(ix + "fraction");

            if (fractions.Any(t => (t.Attribute("name")?.Value ?? "").IndexOf(':') != -1))
                errors.Add("fractionDefinedInExtensionTaxonomy");

            if (fractions.Any())
                errors.Add("fractionElementUsed");

            return errors.Join(",");
        }
    }
}