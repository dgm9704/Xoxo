//
//  InlineXbrlTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2026 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests.Inline
{
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Xml.Linq;
    using Diwen.Xbrl.Inline;
    using Diwen.Xbrl.Xml;
    using Xunit;

    public static class InlineXbrlTests
    {
        [Fact]
        public static void ReadGLEIFAnnualReport()
        {
            var reportPackage = Path.Combine("data", "esma", "gleif-19ar.zip");
            XDocument reportDocument;
            using (var reportStream = File.OpenRead(reportPackage))
            using (var reportArchive = new ZipArchive(reportStream, ZipArchiveMode.Read))
            {
                var reportFile = reportArchive.Entries.FirstOrDefault(e => e.Name == "gleif-19ar.xhtml");
                reportDocument = XDocumentFromZipArchiveEntry(reportFile);
            }

            var report = InlineXbrl.ParseXDocuments(reportDocument);
            Assert.NotNull(report);
            report.ToFile("gleif-19ar.xbrl");
        }

        private static XDocument XDocumentFromZipArchiveEntry(ZipArchiveEntry entry)
        {
            using (var reportStream = entry.Open())
                return XDocument.Load(reportStream);
        }

        [Fact]
        public static void InlineXbrlMultipleSchemaRefs()
        {
            var package = Path.Combine("data", "AR-example.zip");
            XDocument reportDocument;
            using (var reportStream = File.OpenRead(package))
            using (var reportArchive = new ZipArchive(reportStream, ZipArchiveMode.Read))
            {
                var reportFile = reportArchive.Entries.FirstOrDefault(e => e.Name == "AR-example.xhtml");
                reportDocument = XDocumentFromZipArchiveEntry(reportFile);
            }

            var report = InlineXbrl.ParseXDocuments(reportDocument);
            Assert.Equal(3, report.SchemaReferences.Count);

            var outputfile = Path.ChangeExtension(package, "xbrl");
            report.ToFile(outputfile);
            var roundtrip = Report.FromFile(outputfile);
            Assert.Equal(3, roundtrip.SchemaReferences.Count);

        }
    }
}