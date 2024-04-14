namespace Diwen.Xbrl.Csv
{
	using System.Collections.Generic;

	public class ReportData
	{
		public string Table { get; set; }
		public string Datapoint { get; set; }
		public string Value { get; set; }
		public Dictionary<string, string> Dimensions { get; set; } = [];

		public ReportData(string table, string datapoint, string value)
		{
			Table = table;
			Datapoint = datapoint;
			Value = value;
		}

		public ReportData(string table, string datapoint, string value, string dimensionKey, string dimensionValue)
		: this(table, datapoint, value)
		=> Dimensions.Add(dimensionKey, dimensionValue);

		public ReportData(string table, string datapoint, string value, params (string key, string value)[] pairs)
		: this(table, datapoint, value)
		{
			foreach (var pair in pairs)
				Dimensions.Add(pair.key, pair.value);
		}

		public ReportData(string table, string datapoint, string value, Dictionary<string, string> dimensions)
		: this(table, datapoint, value)
		=> Dimensions = dimensions;
	}
}