//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2026 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

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