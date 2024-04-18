namespace Diwen.Xbrl.Csv
{
	using System.Collections.Generic;

	/// <summary/>
	public class ReportData
	{
		/// <summary/>
		public string Table { get; set; }

		/// <summary/>
		public string Datapoint { get; set; }

		/// <summary/>
		public string Value { get; set; }

		/// <summary/>
		public Dictionary<string, string> Dimensions { get; set; } = [];

		/// <summary/>
		public ReportData(string table, string datapoint, string value)
		{
			Table = table;
			Datapoint = datapoint;
			Value = value;
		}

		/// <summary/>
		public ReportData(string table, string datapoint, string value, string dimensionKey, string dimensionValue)
		: this(table, datapoint, value)
		=> Dimensions.Add(dimensionKey, dimensionValue);

		/// <summary/>
		public ReportData(string table, string datapoint, string value, params (string key, string value)[] pairs)
		: this(table, datapoint, value)
		{
			foreach (var pair in pairs)
				Dimensions.Add(pair.key, pair.value);
		}

		/// <summary/>
		public ReportData(string table, string datapoint, string value, Dictionary<string, string> dimensions)
		: this(table, datapoint, value)
		=> Dimensions = dimensions;
	}
}