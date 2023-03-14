namespace Diwen.Xbrl.Package
{
    using System.Text.Json.Serialization;

    public class ReportInfo
    {
        public DocumentInfo documentInfo { get; set; }

        [JsonPropertyName("eba:generatingSoftwareInformation")]
        public EbaGeneratingSoftwareInformation ebageneratingSoftwareInformation { get; set; }
    }
}