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

                foreach (var moduleTable in DocumentInfo.Extends)
                {
                    var tabfile = Path.GetFullPath(Path.Combine(modfolder, moduleTable));
                    if (File.Exists(tabfile))
                        using (var stream = new FileStream(tabfile, FileMode.Open, FileAccess.Read))
                        {
                            var jsonTable = JsonSerializer.Deserialize<TableDefinition>(stream);
                            var tablecode = Path.GetFileNameWithoutExtension(tabfile);
                            tableDefinitions.Add(tablecode, jsonTable);
                        }
                }
            }

            return tableDefinitions;
        }
    }
}