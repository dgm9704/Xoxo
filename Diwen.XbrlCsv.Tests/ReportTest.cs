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

			//datapoint,factValue
			report.AddData("S_00.01", "dp31870", "eba_AS:x1");
			report.AddData("S_00.01", "dp37969", "eba_SC:x6");

			// datapoint,factValue,IRN
			report.AddData("C_105.03", "dp434188", "grarenmw", "IRN", "36");
			report.AddData("C_105.03", "dp434189", "eba_GA:AL", "IRN", "36");
			report.AddData("C_105.03", "dp434188", "grarenmw2", "IRN", "8");
			report.AddData("C_105.03", "dp434189", "eba_GA:AL", "IRN", "8");

			// datapoint,factValue,IMI,PBE
			report.AddData("C_105.02", "dp439585", "250238.28", ("IMI", "ksnpfnwn"), ("PBI", "ksnpfnwn"));
			report.AddData("C_105.02", "dp439586", "247370.72", ("IMI", "ksnpfnwn"), ("PBI", "ksnpfnwn"));
			report.AddData("C_105.02", "dp439585", "250238.28", ("IMI", "kotnyngp"), ("PBI", "kotnyngp"));
			report.AddData("C_105.02", "dp439586", "247370.72", ("IMI", "kotnyngp"), ("PBI", "kotnyngp"));


			// datapoint,factValue,FTY,INC
			report.AddData("C_113.00", "dp439732", "304132.94", new[] { ("FTY", "htkaaxvr"), ("INC", "htkaaxvr") });
			report.AddData("C_113.00", "dp439750", "eba_IM:x33", new[] { ("FTY", "htkaaxvr"), ("INC", "htkaaxvr") });
			report.AddData("C_113.00", "dp439744", "0.1", new[] { ("FTY", "htkaaxvr"), ("INC", "htkaaxvr") });
			report.AddData("C_113.00", "dp439745", "0.72", new[] { ("FTY", "htkaaxvr"), ("INC", "htkaaxvr") });
			report.AddData("C_113.00", "dp439751", "0.34", new[] { ("FTY", "htkaaxvr"), ("INC", "htkaaxvr") });
			report.AddData("C_113.00", "dp439752", "0.46", new[] { ("FTY", "htkaaxvr"), ("INC", "htkaaxvr") });
			report.AddData("C_113.00", "dp439753", "eba_ZZ:x409", new[] { ("FTY", "htkaaxvr"), ("INC", "htkaaxvr") });
			report.AddData("C_113.00", "dp439732", "304132.94", new[] { ("FTY", "ynqtbutq"), ("INC", "ynqtbutq") });
			report.AddData("C_113.00", "dp439750", "eba_IM:x33", new[] { ("FTY", "ynqtbutq"), ("INC", "ynqtbutq") });
			report.AddData("C_113.00", "dp439744", "0.1", new[] { ("FTY", "ynqtbutq"), ("INC", "ynqtbutq") });
			report.AddData("C_113.00", "dp439745", "0.72", new[] { ("FTY", "ynqtbutq"), ("INC", "ynqtbutq") });
			report.AddData("C_113.00", "dp439751", "0.34", new[] { ("FTY", "ynqtbutq"), ("INC", "ynqtbutq") });
			report.AddData("C_113.00", "dp439752", "0.46", new[] { ("FTY", "ynqtbutq"), ("INC", "ynqtbutq") });
			report.AddData("C_113.00", "dp439753", "eba_ZZ:x409", new[] { ("FTY", "ynqtbutq"), ("INC", "ynqtbutq") });

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
