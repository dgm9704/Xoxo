namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Linq;
    using System.Text.Json.Serialization;
    using Diwen.Xbrl.Json;

    /// <summary/>
    public class Table : EsaDocumented
    {

        /// <summary/>
        [JsonPropertyName("optional")]
        public bool Optional { get; set; }

        /// <summary/>
        [JsonPropertyName("template")]
        public string Template { get; set; }

        /// <summary/>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        private string filingIndicator;

        /// <summary />
        [JsonIgnore]
        public string FilingIndicator
        {
            get
            {
                filingIndicator ??=
                    EsaDocumentation != null && EsaDocumentation.ContainsKey("FilingIndicator")
                    ? EsaDocumentation["FilingIndicator"].ToString()
                    : string.Join('.', Template.Split('-').Take(2)); // won't work for EIOPA

                return filingIndicator;
            }
        }
    }
}