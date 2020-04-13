//
//  InlineXbrlTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2020 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml.Linq;
    using Xunit;
    using Xunit.Abstractions;
    using Diwen.Xbrl.Extensions;

    public class InlineXbrlTests
    {

        private readonly ITestOutputHelper output;

        public InlineXbrlTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        public struct Report
        {
            public string Filename { get; }
            public XDocument Document { get; }

            public static readonly Report Empty = new Report(null, null);

            public Report(string filename, XDocument document)
            {
                Filename = filename;
                Document = document;
            }
        }

        public static IEnumerable<object[]> ESEFConformanceSuite()
        {
            var suiteFile = Path.Combine("esma", "esef_conformancesuite_2020-03-06.zip");

            using (var suiteStream = File.OpenRead(suiteFile))
            using (var suiteArchive = new ZipArchive(suiteStream, ZipArchiveMode.Read))
            {
                var suiteIndexFile = suiteArchive.Entries.FirstOrDefault(e => e.Name == "index.xml");

                var suiteIndexDocument = XDocumentFromZipArchiveEntry(suiteIndexFile);

                var testcasesElement = suiteIndexDocument.Root.Descendants("testcases").Single();
                var root = testcasesElement.Attribute("root").Value;
                var title = testcasesElement.Attribute("title").Value;
                var testcaseElements = suiteIndexDocument.Root.Descendants("testcase");
                foreach (var testcaseElement in testcaseElements)
                {
                    var uri = testcaseElement.Attribute("uri").Value;
                    var testcasePath = Path.Combine(root, uri);
                    var testcaseIndexFile = suiteArchive.Entries.FirstOrDefault(e => e.FullName.EndsWith(testcasePath, StringComparison.Ordinal));

                    var testcaseIndexDocument = XDocumentFromZipArchiveEntry(testcaseIndexFile);

                    var ns = testcaseIndexDocument.Root.GetDefaultNamespace();
                    var testcaseNumber = testcaseIndexDocument.Root.Descendants(ns + "number").Single().Value.Trim();
                    var variationElements = testcaseIndexDocument.Root.Descendants(ns + "variation");
                    foreach (var variationElement in variationElements)
                    {
                        var variationId = variationElement.Attribute("id").Value;
                        var resultElement = variationElement.Descendants(ns + "result").Single();
                        var expected = resultElement.Attribute("expected").Value;
                        var expectedResult = expected == "valid";
                        var error = resultElement.Descendants(ns + "error").SingleOrDefault()?.Value.Trim();
                        var packageName = variationElement.Descendants(ns + "taxonomyPackage").Single().Value.Trim();
                        var packagePath = Path.Combine(root, testcaseNumber, packageName);
                        var packageFile = suiteArchive.Entries.SingleOrDefault(e => e.FullName.EndsWith(packagePath, StringComparison.Ordinal));
                        if (packageFile != null)
                        {
                            using (var packageStream = packageFile.Open())
                            using (var packageArchive = new ZipArchive(packageStream, ZipArchiveMode.Read))
                            {
                                // Skipping over any taxonomy stuff 
                                var reportFile = packageArchive.Entries.FirstOrDefault(
                                    e => e.FullName.StartsWith($"{variationId}/reports/", StringComparison.Ordinal)
                                    && e.Name.StartsWith("abc.", StringComparison.Ordinal));

                                if (reportFile != null)
                                {
                                    var report = new Report(reportFile.Name, XDocumentFromZipArchiveEntry(reportFile));
                                    yield return new object[] { testcaseNumber, variationId, expected, error, report };
                                }
                                else
                                {
                                    // this is an actual condition to check for per G2-6 ?                            
                                    // files not in a correct folder
                                    // we're guessing here anyway with finding report files in the variation zip,
                                    // because there isn't and index for it
                                    // TODO: need to figure out how to pass this case on like the others, 
                                    // and have the validation return a meaningful result
                                    yield return new object[] { testcaseNumber, variationId, expected, error, Report.Empty };
                                }
                            }
                        }
                        else
                        {
                            // G3-1-3 incorrect package
                            yield return new object[] { testcaseNumber, variationId, expected, error, Report.Empty };
                        }
                    }
                }
            }
        }

        private static XDocument XDocumentFromZipArchiveEntry(ZipArchiveEntry reportFile)
        {
            using (var reportStream = reportFile.Open())
                return XDocument.Load(reportStream);
        }

        [Theory]
        [MemberData(nameof(ESEFConformanceSuite))]
        public void RunESEFConformanceSuite(string testcaseNumber, string variationId, string expected, string error, Report report)
        {
            // if (testcaseNumber == "G2-4-2_1")
            // {
            var result = InlineXbrl.ValidateEsef(report.Document);
            var expectedError = (error ?? "").Split(',').Select(e => e.Trim()).Join(",");
            var actualError = result.Errors.Join(",");
            output.WriteLine($"{testcaseNumber}\t{variationId}");
            output.WriteLine($"\texpected: {expected} {expectedError}");
            output.WriteLine($"\tactual  : {result.Conclusion} {actualError}");

            var expectedResult = $"{expected} {expectedError}";
            var actualResult = $"{result.Conclusion} {actualError}";
            Assert.Equal(expectedResult, actualResult);
            // }
        }
    }

}