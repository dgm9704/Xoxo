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
    using Diwen.Xbrl.Package;

    /// <summary/>
    public class ModuleDefinition
    {
        /// <summary/>
        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; }

        /// <summary/>
        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        /// <summary/>
        [JsonPropertyName("parameterURL")]
        public string ParameterURL { get; set; }

        /// <summary/>
        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary/>
        [JsonPropertyName("tables")]
        public Dictionary<string, Table> Tables { get; set; }

        /// <summary/>
        public static ModuleDefinition FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<ModuleDefinition>(stream);
        }

        private Dictionary<string, TableDefinition> tableDefinitions;

        /// <summary/>
        public Dictionary<string, TableDefinition> TableDefinitions()
        {
            if (tableDefinitions == null)
            {
                tableDefinitions = [];
                var modfolder = Path.GetDirectoryName(DocumentInfo.Taxonomy.First().ToString().Replace("http://", ""));

                foreach (var moduleTable in DocumentInfo.Extends.Where(f => !f.StartsWith("http://")))
                {
                    var tabfile = Path.GetFullPath(Path.Combine(modfolder, moduleTable));
                    using (var stream = new FileStream(tabfile, FileMode.Open, FileAccess.Read))
                    {
                        var tableDefinition = JsonSerializer.Deserialize<TableDefinition>(stream);
                        var tablecode = tableDefinition.TableTemplates.Single().Key;
                        tableDefinitions.Add(tablecode, tableDefinition);
                    }
                }
            }

            return tableDefinitions;
        }

        private Dictionary<string, Filing> filingInfo;

        /// <summary /> 
        public Dictionary<string, Filing> FilingInfo()
        {
            if (filingInfo == null)
            {
                filingInfo = [];

                foreach (var table in this.Tables.Values)
                {
                    switch (table.Template)
                    {
                        case "FootNotes":
                        case "FilingIndicators":
                            break;

                        default:
                            filingInfo.Add(
                                table.Template,
                                new Filing
                                {
                                    Template = table.Template,
                                    Url = table.Url,
                                    Indicator = table.FilingIndicator,
                                });

                            break;
                    }
                }
            }

            return filingInfo;
        }

    }
}