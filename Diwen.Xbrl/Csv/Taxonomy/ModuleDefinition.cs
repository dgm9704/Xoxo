namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;

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
                var modfolder = Path.GetDirectoryName(DocumentInfo.Taxonomy.First().Replace("http://", ""));

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

        private List<FilingIndicatorInfo> filingIndicatorInfos;

        /// <summary /> 
        public List<FilingIndicatorInfo> FilingIndicatorInfos()
        {
            if (filingIndicatorInfos == null)
            {
                filingIndicatorInfos = [];
                foreach (var table in this.Tables)
                {
                    switch (table.Value.Template)
                    {
                        case "FootNotes":
                        case "FilingIndicators":
                            break;

                        default:
                            filingIndicatorInfos.Add(
                                new FilingIndicatorInfo
                                {
                                    TableCode = table.Key,
                                    TemplateCode = table.Value.Template,
                                    Url = table.Value.Url,
                                    FilingIndicatorCode = table.Value.EbaDocumentation.Any()
                                        ? table.Value.EbaDocumentation["FilingIndicator"].ToString()
                                        : string.Join('.', table.Value.Template.Split('-').Take(2)),
                                });

                            break;
                    }
                }



            }

            return filingIndicatorInfos;
        }

    }
}