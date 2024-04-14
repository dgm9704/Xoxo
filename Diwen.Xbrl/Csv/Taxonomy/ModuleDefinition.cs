namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Diwen.Xbrl.Package;

    public class ModuleDefinition
    {

        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; }

        [JsonPropertyName("documentInfo")]
        public DocumentInfo DocumentInfo { get; set; }

        [JsonPropertyName("parameterURL")]
        public string ParameterURL { get; set; }

        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; }

        [JsonPropertyName("tables")]
        public Dictionary<string, Table> Tables { get; set; }

        public static ModuleDefinition FromFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<ModuleDefinition>(stream);
        }

        private Dictionary<string, TableDefinition> tableDefinitions;

        public Dictionary<string, TableDefinition> TableDefinitions()
        {
            if (tableDefinitions == null)
            {
                tableDefinitions = [];
                var modfolder = Path.GetDirectoryName(DocumentInfo.taxonomy.First().Replace("http://", ""));

                foreach (var moduleTable in DocumentInfo.extends)
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