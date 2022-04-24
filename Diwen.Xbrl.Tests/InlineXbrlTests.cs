namespace Diwen.Xbrl.Tests
{
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Xml.Linq;
	using Diwen.Xbrl.Inline;
	using Xunit;

	public static class InlineXbrlTests
	{
		[Fact]
		public static void ReadGLEIFAnnualReport()
		{
			var reportPackage = Path.Combine("esma", "gleif-19ar.zip");

			using (var reportStream = File.OpenRead(reportPackage))
			using (var reportArchive = new ZipArchive(reportStream, ZipArchiveMode.Read))
			{
				var reportFile = reportArchive.Entries.FirstOrDefault(e => e.Name == "gleif-19ar.xhtml");
				var reportDocument = XDocumentFromZipArchiveEntry(reportFile);

				// var reportfile = Path.Combine("esma", "gleif-19ar.xhtml");
				//var instance = InlineXbrl.ParseFiles(reportfile);
				var instance = InlineXbrl.ParseXDocuments(reportDocument);
				Assert.NotNull(instance);
				instance.ToFile("gleif-19ar.xbrl");
			}
		}

		private static XDocument XDocumentFromZipArchiveEntry(ZipArchiveEntry entry)
		{
			using (var reportStream = entry.Open())
				return XDocument.Load(reportStream);
		}
	}
}