// //
// //  InlineXbrlTests.cs
// //
// //  Author:
// //       John Nordberg <john.nordberg@gmail.com>
// //
// //  Copyright (c) 2015-2024 John Nordberg
// //
// //  Free Public License 1.0.0
// //  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
// //  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
// //  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
// //  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
// //  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

// namespace Diwen.Xbrl.Tests
// {
//     using System;
//     using System.Collections.Generic;
//     using System.IO;
//     using System.IO.Compression;
//     using System.Linq;
//     using System.Xml.Linq;
//     using Xunit;
//     using Xunit.Abstractions;
//     using Diwen.Xbrl.Extensions;
//     using Diwen.Xbrl;

//     public class EsefConformanceTests
//     {

//         private readonly ITestOutputHelper output;

//         public EsefConformanceTests(ITestOutputHelper output)
//         {
//             this.output = output;
//         }

//         public static IEnumerable<object[]> EsefConformanceSuite()
//         {
//             var suiteFile = Path.Combine("esma", "esef_conformancesuite_2020-03-06.zip");

//             using (var suiteStream = File.OpenRead(suiteFile))
//             using (var suiteArchive = new ZipArchive(suiteStream, ZipArchiveMode.Read))
//             {
//                 var suiteIndexFile = suiteArchive.Entries.FirstOrDefault(e => e.Name == "index.xml");

//                 var suiteIndexDocument = XDocumentFromZipArchiveEntry(suiteIndexFile);

//                 var testcasesElement = suiteIndexDocument.Root.Descendants("testcases").Single();
//                 var root = testcasesElement.Attribute("root").Value;
//                 var title = testcasesElement.Attribute("title").Value;
//                 var testcaseElements = suiteIndexDocument.Root.Descendants("testcase");
//                 foreach (var testcaseElement in testcaseElements)
//                 {
//                     var uri = testcaseElement.Attribute("uri").Value;
//                     var testcasePath = Path.Combine(root, uri);
//                     var testcaseIndexFile = suiteArchive.Entries.FirstOrDefault(e => e.FullName.EndsWith(testcasePath, StringComparison.Ordinal));

//                     var testcaseIndexDocument = XDocumentFromZipArchiveEntry(testcaseIndexFile);

//                     var ns = testcaseIndexDocument.Root.GetDefaultNamespace();
//                     var testcaseNumber = testcaseIndexDocument.Root.Descendants(ns + "number").Single().Value.Trim();
//                     var variationElements = testcaseIndexDocument.Root.Descendants(ns + "variation");
//                     foreach (var variationElement in variationElements)
//                     {
//                         var variationId = variationElement.Attribute("id").Value;
//                         var resultElement = variationElement.Descendants(ns + "result").Single();
//                         var expected = resultElement.Attribute("expected").Value;
//                         var expectedResult = expected == "valid";
//                         var error = resultElement.Descendants(ns + "error").SingleOrDefault()?.Value.Trim();
//                         var packageName = variationElement.Descendants(ns + "taxonomyPackage").Single().Value.Trim();
//                         var packagePath = Path.Combine(root, testcaseNumber, packageName);
//                         var packageFile = suiteArchive.Entries.SingleOrDefault(e => e.FullName.EndsWith(packagePath, StringComparison.Ordinal));
//                         var report = new List<ReportFile>();
//                         if (packageFile != null)
//                         {
//                             using (var packageStream = packageFile.Open())
//                             using (var packageArchive = new ZipArchive(packageStream, ZipArchiveMode.Read))
//                             {
//                                 // Skipping over any taxonomy stuff 
//                                 var reportFiles = packageArchive.Entries.
//                                     Where(e => e.Length != 0).
//                                     Where(e => e.FullName.StartsWith($"{variationId}/reports/", StringComparison.Ordinal));

//                                 foreach (var reportFile in reportFiles)
//                                     report.Add(new ReportFile(reportFile.Name, (ContentFromZipArchiveEntry(reportFile))));

//                                 yield return new object[] { testcaseNumber, variationId, expected, error, report };
//                             }
//                         }
//                         else
//                         {
//                             // G3-1-3 incorrect package
//                             yield return new object[] { testcaseNumber, variationId, expected, error, report };
//                         }
//                     }
//                 }
//             }
//         }

//         private static object ContentFromZipArchiveEntry(ZipArchiveEntry entry)
//         {
//             var extension = Path.GetExtension(entry.Name);
//             return (extension == ".html" || extension == ".xhtml")
//                 ? XDocumentFromZipArchiveEntry(entry)
//                 : (object)ByteArrayFromZipArchiveEntry(entry);
//         }

//         private static byte[] ByteArrayFromZipArchiveEntry(ZipArchiveEntry entry)
//         {
//             using (var reportStream = entry.Open())
//             using (var memoryStream = new MemoryStream())
//             {
//                 reportStream.CopyTo(memoryStream);
//                 return memoryStream.ToArray();
//             }
//         }

//         private static XDocument XDocumentFromZipArchiveEntry(ZipArchiveEntry entry)
//         {
//             using (var reportStream = entry.Open())
//                 return XDocument.Load(reportStream);
//         }

//         [Theory]
//         [MemberData(nameof(EsefConformanceSuite))]
//         public void RunEsefConformanceSuite(string testcaseNumber, string variationId, string expected, string error, IEnumerable<ReportFile> report)
//         {
//             // if (testcaseNumber == "G2-5-1")
//             // {
//             var result = EsefReportingManual.Validate(report);
//             var expectedError = (error ?? "").Split(',').Select(e => e.Trim()).Join(",");
//             var actualError = result.Errors.Join(",");
//             output.WriteLine($"{testcaseNumber}\t{variationId}");
//             output.WriteLine($"\texpected: {expected} {expectedError}");
//             output.WriteLine($"\tactual  : {result.Conclusion} {actualError}");

//             var expectedResult = $"{expected} {expectedError}";
//             var actualResult = $"{result.Conclusion} {actualError}";
//             Assert.Equal(expectedResult, actualResult);
//             // }
//         }
//     }

// }