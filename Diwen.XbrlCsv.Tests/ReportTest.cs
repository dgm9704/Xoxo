namespace Diwen.XbrlCsv.Tests
{
	using System.IO;
	using System.Text.Json;
	using Xunit;

	public class ReportTest
	{
		[Fact]
		public void Export()
		{
			var report = new Report();
			report.DocumentInfo.Extends = "http://www.eba.europa.eu/eu/fr/xbrl/crr/fws/sbp/cir-2070-2016/2021-07-15/mod/sbp_cr_con.json";

			report.Parameters.Add("entityID", "lei:DUMMYLEI123456789012");
			report.Parameters.Add("refPeriod", "2021-12-31");
			report.Parameters.Add("baseCurrency", "iso4217:EUR");
			report.Parameters.Add("decimalsInteger", "0");
			report.Parameters.Add("decimalsMonetary", "-3");
			report.Parameters.Add("decimalsPercentage", "4");
			report.Parameters.Add("decimalsDecimal", "2");

			report.FilingIndicators.Add("C_101.00", true);
			report.FilingIndicators.Add("C_102.00", true);
			report.FilingIndicators.Add("C_103.00", true);
			report.FilingIndicators.Add("C_105.01", true);
			report.FilingIndicators.Add("C_105.02", true);
			report.FilingIndicators.Add("C_105.03", true);
			report.FilingIndicators.Add("C_111.00", true);
			report.FilingIndicators.Add("C_112.00", true);
			report.FilingIndicators.Add("C_113.00", true);
			report.FilingIndicators.Add("C_114.00", true);
			report.FilingIndicators.Add("S_00.01", true);

			report.Export();
		}

		[Fact]
		public void Import()
		{
			string fileName = "report.json";
			string jsonString = File.ReadAllText(fileName);
			var report = JsonSerializer.Deserialize<Report>(jsonString);
		}
	}
}
