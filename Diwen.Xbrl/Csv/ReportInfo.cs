namespace Diwen.Xbrl.Csv
{
    using System.Text.Json.Serialization;

    public partial class Report
    {
        public class ReportInfo
        {
            public DocumentInfo documentInfo { get; set; }

            [JsonPropertyName("eba:generatingSoftwareInformation")]
            public EbaGeneratingSoftwareInformation ebageneratingSoftwareInformation { get; set; }
        }
    }
}