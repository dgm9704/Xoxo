namespace Diwen.Xbrl.Json
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary/>
    public class EsaDocumented
    {
        private Dictionary<string, object> esaDocumentation;

        /// <summary/>
        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtensionData { get; set; }

        /// <summary/>
        [JsonIgnore]
        public Dictionary<string, object> EsaDocumentation
        {
            get
            {
                esaDocumentation ??=
                    ExtensionData != null
                    ? JsonSerializer.Deserialize<Dictionary<string, object>>(ExtensionData["eba:documentation"])
                    : [];

                return esaDocumentation;
            }
        }
    }
}