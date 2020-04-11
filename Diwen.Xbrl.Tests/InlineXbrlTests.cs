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

    public static class InlineXbrlTests
    {

        // [Fact]
        // public static void ParseInlineXbrlDocument()
        // {
        //     var inputFile = "esma/G2-1-2.xhtml";
        //     var instance = InlineXbrl.ParseInstance(inputFile);
        //     var outputFile = Path.ChangeExtension(inputFile, "xbrl");
        //     instance.ToFile(outputFile);
        // }

        // [Fact]
        // public static void ValidateInlineXbrlDocument()
        // {
        //     var inputFile = "esma/G2-1-2.xhtml";
        //     var result = InlineXbrl.ValidateEsef(inputFile);
        //     Assert.False(result.Messages.Any(), string.Join("\n", result.Messages));
        // }

        // [Fact]
        // public static void ValidateUnknownInlineXbrlDocument()
        // => Assert.Throws<ArgumentOutOfRangeException>(
        //     () => InlineXbrl.ValidateEsef("inputFile", (InlineXbrlType)42));

        public static IEnumerable<object[]> ESEFConformanceSuite()
        {
            var results = new List<object[]> { };
            var zipFile = Path.Combine("esma", "esef_conformancesuite_2020-03-06.zip");
            using (var file = File.OpenRead(zipFile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                var indexFile = zip.Entries.FirstOrDefault(e => e.Name == "index.xml");
                var index = XDocument.Load(indexFile.Open()); // TODO: using
                var testcasesElement = index.Root.Descendants("testcases").Single();
                var root = testcasesElement.Attribute("root").Value;
                var title = testcasesElement.Attribute("title").Value;
                var testcaseElements = index.Root.Descendants("testcase");
                foreach (var testcaseElement in testcaseElements)
                {
                    var uri = testcaseElement.Attribute("uri").Value;
                    var testcasePath = Path.Combine(root, uri);
                    var testcaseIndexFile = zip.Entries.FirstOrDefault(e => e.FullName.EndsWith(testcasePath, StringComparison.Ordinal));
                    var testcaseIndex = XDocument.Load(testcaseIndexFile.Open()); // TODO: using
                    var ns = testcaseIndex.Root.GetDefaultNamespace();
                    var testcaseNumber = testcaseIndex.Root.Descendants(ns + "number").Single().Value;
                    var variationElements = testcaseIndex.Root.Descendants(ns + "variation");
                    foreach (var variationElement in variationElements)
                    {
                        var variationId = variationElement.Attribute("id").Value;
                        var resultElement = variationElement.Descendants(ns + "result").Single();
                        var expected = resultElement.Attribute("expected").Value;
                        var expectedResult = expected == "valid";
                        var error = resultElement.Descendants(ns + "error").SingleOrDefault()?.Value;
                        var packageName = variationElement.Descendants(ns + "taxonomyPackage").Single().Value;
                        var packagePath = Path.Combine(root, testcaseNumber, packageName);
                        var packageFile = zip.Entries.SingleOrDefault(e => e.FullName.EndsWith(packagePath, StringComparison.Ordinal));
                        if (packageFile != null)
                        {
                            var package = new ZipArchive(packageFile.Open(), ZipArchiveMode.Read); // TODO: using using
                                                                                                   // Skipping over any taxonomy stuff 
                            var reportFile = package.Entries.FirstOrDefault(
                                e => e.FullName.StartsWith($"{variationId}/reports/", StringComparison.Ordinal)
                                && e.Name.StartsWith("abc.", StringComparison.Ordinal));

                            if (reportFile != null)
                            {
                                var reportFilename = reportFile.Name;
                                var report = XDocument.Load(reportFile.Open()); // TODO: using
                                                                                //yield return new object[] { reportFilename, expectedResult, report, };
                                results.Add(new object[] { testcaseNumber, variationId, expectedResult, reportFilename, report, });
                            }
                            else
                            {
                                // this is an actual condition to check for per G2-6 ?                            
                                // files not in a correct folder
                                // we're guessing here anyway with finding report files in the variation zip,
                                // because there isn't and index for it
                                // TODO: need to figure out how to pass this case on like the others, 
                                // and have the validation return a meaningful result
                                results.Add(new object[] { testcaseNumber, variationId, expectedResult, string.Empty, null, });
                            }
                        }
                        else
                        {
                            // G3-1-3 incorrect package
                            results.Add(new object[] { testcaseNumber, variationId, expectedResult, string.Empty, null, });
                        }
                    }
                }
            }
            return results;
        }

        [Theory]
        [MemberData(nameof(ESEFConformanceSuite))]
        public static void RunESEFConformanceSuite(string testcaseNumber, string variationId, bool expectedResult, string reportFilename, XDocument report)
        {
            Console.Write($"{testcaseNumber}\t{variationId}\texpected: {expectedResult}");
            var result = InlineXbrl.ValidateEsef(report);
            Console.WriteLine($"\tactual: {result.Success}");
            Assert.Equal(expectedResult, result.Success); //, result.Messages.Any(), string.Join("\n", result.Messages));
        }

    }

}