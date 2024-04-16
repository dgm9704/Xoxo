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
    using System.Linq;
    using System.Xml.Linq;
    using Diwen.Xbrl.Extensions;

    public static class EsefReportingManual
    {

        private static List<Func<IEnumerable<ReportFile>, string>> EsefValidations =
        new List<Func<IEnumerable<ReportFile>, string>>
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
            G2_4_2_1,
            G2_4_2_2,
            G2_5_1,
            G2_5_2,
            G2_5_3,
            G2_5_4_1,
            G2_5_4_2,
            G2_6_1,
            G2_6_2,
            G2_7_1_1,
            G2_7_1_2,
            G3_1_1_1,
            G3_1_1_2,
            G3_1_1_3,
            G3_2_1,
            G3_2_2,
            G3_2_3,
            G3_2_5,
            G3_4_2_1,
            G3_4_2_2,
            G3_4_2_3,
            G3_4_2_4,
            G3_4_3_1,
            G3_4_3_2,
            G3_4_4,
            G3_4_5_1,
            G3_4_5_2,
            G3_5_1
        };

        public static EsefResult Validate(IEnumerable<ReportFile> reportFiles)
        {
            var errors =
                EsefValidations.
                Select(validation => validation(reportFiles)).
                Where(error => !string.IsNullOrWhiteSpace(error)).
                ToArray();

            return new EsefResult(errors.Any() ? "invalid" : "valid", errors.ToArray());
        }


        private static string G2_1_2(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();
            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;
                if (document != null)
                {
                    var xbrli = document.Root.GetNamespaceOfPrefix("xbrli");
                    var periodElements = document.Root.Descendants(xbrli + "period");
                    var dates = periodElements.SelectMany(p => p.Descendants().Select(d => d.Value));

                    if (dates.Any(d => d.IndexOf('T') != -1))
                        errors.Add("periodWithTimeContent");

                    var zones = new char[] { 'Z', '+', '-' };
                    if (dates.Any(d => d.LastIndexOfAny(zones) > 9))
                        errors.Add("periodWithTimeZone");
                }
            }

            return errors.Join(",");
        }

        private static string G2_1_3_1(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();
            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;
                var xbrli = document.Root.GetNamespaceOfPrefix("xbrli");
                var segmentElements = document.Root.Descendants(xbrli + "segment");
                if (segmentElements.Any())
                    errors.Add("segmentUsed");
            }
            return errors.Join(",");
        }

        private static string G2_1_3_2(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();
            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;
                var xbrli = document.Root.GetNamespaceOfPrefix("xbrli");
                var xbrldi = document.Root.GetNamespaceOfPrefix("xbrldi");
                var scenarioElements = document.Root.Descendants(xbrli + "scenario");
                var customElements = scenarioElements.SelectMany(s => s.Descendants().Where(e => e.Name.Namespace != xbrldi));

                if (customElements.Any())
                    errors.Add("scenarioContainsNonDimensionalContent");
            }
            return errors.Join(",");
        }

        private static string G2_2_1(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();
            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;
                var factElements = InlineXbrl.FindFacts(document);

                if (factElements.Any(e => e.Attribute("precision") != null))
                    errors.Add("precisionAttributeUsed");
            }
            return errors.Join(",");
        }

        private static string G2_2_2(IEnumerable<ReportFile> reportFiles)
        {
            // var num = report.Root.GetNamespaceOfPrefix("num");
            // var percents = report.Root.Descendants(num + "percentItemType");
            // return percents.Any(e => Decimal.Parse(e.Value) > 1)
            //         ? "All elements of num:percentItemType should be less or equal to 1"
            //         : null;

            // checking this requires loading documents outside the report
            return null;
        }

        private static string G2_2_3(IEnumerable<ReportFile> reportFiles)
        {

            var errors = new HashSet<string>();
            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;

                var ixt = document.Root.GetNamespaceOfPrefix("ixt");
                if (!(
                    ixt == null
                    || ixt.NamespaceName == "http://www.xbrl.org/inlineXBRL/transformation/2015-02-26" // TR 3
                    || ixt.NamespaceName == "http://www.xbrl.org/inlineXBRL/transformation/2019-04-19" // TR 4 PWD
                    || ixt.NamespaceName == "http://www.xbrl.org/inlineXBRL/transformation/2020-02-12" // TR 4
                ))
                    errors.Add("transformRegistry");
            }
            return errors.Join(",");
        }

        private static string G2_3_1_1(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();
            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;

                var ix = document.Root.GetNamespaceOfPrefix("ix");
                var footnotes = document.Root.Descendants(ix + "footnote");
                var relationships = document.Root.Descendants(ix + "relationship");

                if (footnotes.Any(f => f.Attribute("footnoteRole") != null)
                    || relationships.Any(r => r.Attribute("arcrole") != null))
                    errors.Add("nonStandardRoleForFootnote");
            }
            return errors.Join(",");
        }

        private static string G2_3_1_2(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            var footnotes = new HashSet<string>();
            var relationships = new HashSet<string>();
            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;

                var ix = document.Root.GetNamespaceOfPrefix("ix");

                document.Root.
                    Descendants(ix + "footnote").
                    Select(f => f.Attribute("id")?.Value).
                    Where(a => !string.IsNullOrEmpty(a)).
                    ToList().
                    ForEach(f => footnotes.Add(f));

                document.Root.
                    Descendants(ix + "relationship").
                    Select(r => r.Attribute("toRefs")?.Value).
                    Where(a => !string.IsNullOrEmpty(a)).
                    ToList().
                    ForEach(r => relationships.Add(r));
            }

            if (footnotes.Except(relationships).Any())
                errors.Add("unusedFootnote");

            return errors.Join(",");

        }

        private static string G2_3_1_3(IEnumerable<ReportFile> reportFiles)
        {
            var error = new HashSet<string>();
            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;

                var ix = document.Root.GetNamespaceOfPrefix("ix");
                var xml = document.Root.GetNamespaceOfPrefix("xml");

                var reportLanguage = document.Root.Attribute(xml + "lang")?.Value ?? "";

                var footnotes =
                    document.Root.
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
            }
            return error.Join(",");
        }

        private static string G2_4_1_1(IEnumerable<ReportFile> reportFiles)
        {
            var error = new HashSet<string>();
            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;
                var ix = document.Root.GetNamespaceOfPrefix("ix");

                var hiddenFacts =
                    document.Root.
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
                    document.Root.
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
            }
            return error.Join(",");
        }

        private static string G2_4_1_2(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;

                var ix = document.Root.GetNamespaceOfPrefix("ix");
                var tuples = document.Root.Descendants(ix + "tuple");

                if (tuples.Any(t => (t.Attribute("name")?.Value ?? "").IndexOf(':') != -1))
                    errors.Add("tupleDefinedInExtensionTaxonomy");

                if (tuples.Any())
                    errors.Add("tupleElementUsed");
            }
            return errors.Join(",");
        }

        private static string G2_4_1_3(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;

                var ix = document.Root.GetNamespaceOfPrefix("ix");
                var fractions = document.Root.Descendants(ix + "fraction");

                if (fractions.Any(t => (t.Attribute("name")?.Value ?? "").IndexOf(':') != -1))
                    errors.Add("fractionDefinedInExtensionTaxonomy");

                if (fractions.Any())
                    errors.Add("fractionElementUsed");
            }
            return errors.Join(",");
        }

        private static string G2_4_2_1(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;
                var xml = document.Root.GetNamespaceOfPrefix("xml");
                if (document.Root.DescendantsAndSelf().Any(e => e.Attribute(xml + "base") != null))
                    errors.Add("htmlOrXmlBaseUsed");
            }
            return errors.Join(",");
        }

        private static string G2_4_2_2(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;
                var html = document.Root.GetDefaultNamespace();

                if (document.Root.Descendants(html + "base").Any())
                    errors.Add("htmlOrXmlBaseUsed");
            }
            return errors.Join(",");
        }

        private static string G2_5_1(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;
                var html = document.Root.GetDefaultNamespace();
                if (document.Root.
                    Descendants(html + "img").
                    Select(i => i.Attribute("src").Value).
                    Where(src => !src.StartsWith("http")). // external reference handled elsewhere
                    Any(src => !src.StartsWith("data:") || src.IndexOf(";base64,") == -1))
                    errors.Add("embeddedImageNotUsingBase64Encoding");
            }
            return errors.Join(",");
        }

        private static string G2_5_2(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;

                var ix = document.Root.GetNamespaceOfPrefix("ix");
                var xml = document.Root.GetNamespaceOfPrefix("xml");

                var reportLanguage =
                    document.Root.
                    Attribute(xml + "lang")?.
                    Value;

                var textFactLanguages =
                    document.Root.
                    Descendants(ix + "nonNumeric").
                    Select(f =>
                        f.Attribute(xml + "lang")?.Value ?? f.Parent.Attribute(xml + "lang")?.Value).
                    ToHashSet();

                if (string.IsNullOrEmpty(reportLanguage))
                {
                    if (textFactLanguages.Any(l => string.IsNullOrEmpty(l)))
                        errors.Add("undefinedLanguageForTextFact");
                }
                else
                {
                    if (textFactLanguages.Where(l => !string.IsNullOrEmpty(l)).Any(l => l != reportLanguage))
                        errors.Add("taggedTextFactOnlyInLanguagesOtherThanLanguageOfAReport");
                }
            }
            return errors.Join(",");
        }

        private static string G2_5_3(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            foreach (var reportFile in reportFiles.Where(f => f.Content is XDocument))
            {
                var document = reportFile.Content as XDocument;

                var ix = document.Root.GetNamespaceOfPrefix("ix");
                if (document.Root.
                    Descendants().
                    Where(e => e.Name.Namespace == ix).
                    Any(e => e.Attribute("target") != null))
                    errors.Add("targetAttributeUsed");
            }
            return errors.Join(",");
        }

        private static string G2_5_4_1(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            var parts = reportFiles.Where(f => f.Content is XDocument).ToArray();
            if (parts.Length == 1)
            {
                var document = parts.Single().Content as XDocument;
                var html = document.Root.GetDefaultNamespace();
                if (
                    document.Root.
                    Descendants(html + "link").
                    Any(l => l.Attributes("rel").
                    Any(r => r.Value == "stylesheet")))
                    errors.Add("externalCssFileForSingleIXbrlDocument");
            }
            return errors.Join(",");
            //Where an Inline XBRL document set contains a single document, 
            //the CSS MUST be embedded within the document. 
        }

        private static string G2_5_4_2(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            var parts = reportFiles.Where(f => f.Content is XDocument).ToArray();
            if (parts.Length > 1)
            {
                foreach (var part in parts)
                {
                    var document = part.Content as XDocument;
                    var html = document.Root.GetDefaultNamespace();
                    if (document.Root.
                        Descendants(html + "style").
                        Any())
                        errors.Add("embeddedCssForMultiHtmlIXbrlDocumentSets");
                }
            }
            return errors.Join(",");

            //Where an Inline XBRL document set contains multiple documents, 
            //the CSS SHOULD be defined in a separate file.

        }

        private static string G2_6_1(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G2_6_2(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G2_7_1_1(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G2_7_1_2(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_1_1_1(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_1_1_2(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_1_1_3(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_2_1(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_2_2(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_2_3(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_2_5(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_4_2_1(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_4_2_2(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_4_2_3(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_4_2_4(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_4_3_1(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_4_3_2(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_4_4(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_4_5_1(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_4_5_2(IEnumerable<ReportFile> reportFiles)
        => null;

        private static string G3_5_1(IEnumerable<ReportFile> reportFiles)
        {
            var errors = new HashSet<string>();

            var parts = reportFiles.Where(f => f.Content is XDocument).ToArray();
            foreach (var part in parts)
            {
                var document = part.Content as XDocument;
                var html = document.Root.GetDefaultNamespace();
                if (document.Root.
                    Descendants().
                    Any(d => (d.Attribute("src")?.Value ?? "").StartsWith("http")))
                    errors.Add("inlinXbrlContainsExternalReferences"); // Typo in the conformance test data
            }
            return errors.Join(",");
        }


    }
}