namespace Diwen.Xbrl.Csv.Tests
{
    using System.Linq;
    using Diwen.Xbrl.Json;
    using Xunit;
    using Xunit.Abstractions;

    public class XbrlJsonTests
    {
        private readonly ITestOutputHelper output;

        public XbrlJsonTests(ITestOutputHelper output)
        => this.output = output;

        [Theory]
        [InlineData("json/example.json")]
        public static void ImportTest(string reportPath)
        {
            var report = Report.FromFile(reportPath);
            Assert.True(report.Facts.Any());
        }
    }
}