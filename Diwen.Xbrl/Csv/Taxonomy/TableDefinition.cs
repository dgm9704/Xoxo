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

namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Diwen.Xbrl.Extensions;
    using Diwen.Xbrl.Package;

    /// <summary/>
    public class TableDefinition
    {

        /// <summary/>
        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        /// <summary/>
        [JsonPropertyName("tableTemplates")]
        public Dictionary<string, TableTemplate> TableTemplates { get; set; }

        private Dictionary<string, PropertyGroup> datapoints;

        /// <summary/>
        public Dictionary<string, PropertyGroup> Datapoints
        {
            get
            {
                datapoints ??= TableTemplates.First().Value.Columns.Datapoint.PropertyGroups;
                return datapoints;
            }
        }

        private Dictionary<string, Column> columns;

        /// <summary/>
        public Dictionary<string, Column> Columns
        {
            get
            {
                columns ??= TableTemplates.First().Value.Columns.TableColumns;
                return columns;
            }
        }


        private Dictionary<string, KeyValuePair<string, PropertyGroup>[]> datapointsByMetric;
        private Dictionary<string, KeyValuePair<string, Column>[]> columnsByMetric;

        private static readonly KeyValuePair<string, PropertyGroup>[] noCandidates = [];

        /// <summary/>
        public KeyValuePair<string, PropertyGroup>[] GetDatapointsByMetric(string metric)
        {
            if (datapointsByMetric == null)
            {
                datapointsByMetric =
                    Datapoints.
                    GroupBy(pg => pg.Value.Dimensions["concept"]).
                    ToDictionary(
                        g => g.Key,
                        g => g.ToArray());
            }

            return datapointsByMetric.GetValueOrDefault(metric, noCandidates);
        }

        /// <summary/>
        public KeyValuePair<string, Column>[] GetColumnsByMetric(string metric)
        {
            columnsByMetric ??=
                    Columns.
                    Where(c => c.Value.Dimensions.ContainsKey("concept")).
                    GroupBy(c => c.Value.Dimensions["concept"]).
                    ToDictionary(
                        g => g.Key,
                        g => g.ToArray());

            return columnsByMetric.GetValueOrDefault(metric, []);
        }

        /// <summary/>
        public static TableDefinition FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<TableDefinition>(stream);
        }
    }
}