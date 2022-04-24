namespace Diwen.Xbrl.Tests
{
	using System.IO;
	using Diwen.Xbrl.Inline;
	using Xunit;

	public static class InlineXbrlTests
	{
		[Fact]
		public static void ReadGLEIFAnnualReport()
		{
			var reportfile = Path.Combine("esma", "gleif-19ar.xhtml");
			var instance = InlineXbrl.ParseFiles(reportfile);
			Assert.NotNull(instance);
			instance.ToFile("gleif-19ar.xbrl");
		}
	}
}