using System.IO;
using System.Threading.Tasks;
using Diwen.Xbrl.Csv.Taxonomy;
using Diwen.Xbrl.Extensions;
using Xunit;

namespace Diwen.Xbrl.Tests.Csv
{
    public class Eiopa
    {
        [Theory]
        [InlineData("data/eiopa/2.10/spv_2026-06-30_instance.xbrl", @"eiopa/2.10")]
        public async Task XmlToCsv(string xmlReportPath, string outputFolderPath)
        {
            var xml = Diwen.Xbrl.Xml.Report.FromFile(xmlReportPath);
            var xmlSchemaRef = xml.SchemaReference;
            var csvSchemaPath = Path.Combine("taxonomy", Path.ChangeExtension(xmlSchemaRef.Value.Replace("http://", string.Empty), ".json"));
            var moduleDefinition = ModuleDefinition.FromFile(csvSchemaPath);
            var csv = xml.ToXbrlCsv(moduleDefinition);
            var csvReportName = Path.ChangeExtension(Path.GetFileName(xmlReportPath), "zip");
            var csvReportPath = Path.Combine(outputFolderPath, csvReportName);
            csv.Export(csvReportPath, moduleDefinition);
        }
    }
}