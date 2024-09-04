namespace Diwen.Xbrl.Tests.Inline
{
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Xml.Linq;
    using Diwen.Xbrl.Inline;
    using Xunit;

    public static class InlineXbrlTests
    {
        [Fact]
        public static void ReadGLEIFAnnualReport()
        {
            var reportPackage = Path.Combine("data/esma", "gleif-19ar.zip");
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
            var reportfile = Path.Combine("data", "AR-example.xhtml");
            var report = InlineXbrl.ParseFiles(reportfile);
            report.ToFile(Path.ChangeExtension(reportfile, "xbrl"));
        }
    }
}