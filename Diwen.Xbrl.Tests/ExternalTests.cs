//
//  ExternalTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2020 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using Diwen.Xbrl.Xml;
    using Diwen.Xbrl.Xml.Comparison;
    using Xunit;
    using Xunit.Abstractions;

    public class ExternalTests
    {
        private readonly ITestOutputHelper output;

        public ExternalTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void EBA()
        {
            var folder = "eba";
            foreach (var zip in Directory.GetFiles(folder, "*.zip"))
                TestZippedFiles(zip, folder);

        }

        [Fact]
        public void EIOPA()
        {
            var folder = "eiopa";
            foreach (var zip in Directory.GetFiles(folder, "*.zip"))
                TestZippedFiles(zip, folder);

        }

        // [Fact]
        // public static void Fi_Sbr()
        // => CheckFolderResults(TestFolder("fi-sbr"));

        void CheckFolderResults(Dictionary<string, ComparisonReport> reports)
        => Assert.True(
                reports.Values.All(report => report.Result),
                string.Join(Environment.NewLine,
                    reports.
                    Where(report => !report.Value.Result).
                    Select(report => report.Key)));

        Dictionary<string, ComparisonReport> TestFolder(string folderName)
        => Directory.GetFiles(folderName, "*.xbrl").
                ToDictionary(inputFile => inputFile,
                            inputFile => TestFile(inputFile,
                                                Path.ChangeExtension(inputFile, "out"),
                                                Path.ChangeExtension(inputFile, "log")));


        ComparisonReport TestFile(string inputFile, string outputFile, string reportFile)
        {
            output.WriteLine(Path.GetFileName(inputFile));
            Instance.FromFile(inputFile).ToFile(outputFile);
            var report = InstanceComparer.Report(inputFile, outputFile);
            File.WriteAllLines(reportFile, report.Messages);
            return report;
        }

        List<ComparisonReport> TestZippedFiles(string zipFile, string outputFolder)
        {
            output.WriteLine(zipFile);
            var result = new List<ComparisonReport>();
            using (var file = File.OpenRead(zipFile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries.Where(e => Path.GetExtension(e.Name) == ".xbrl"))
                {
                    output.WriteLine($"\t{entry.Name}");
                    var outputFile = Path.Combine(outputFolder, Path.ChangeExtension(entry.Name, "out"));

                    var memoryStream = new MemoryStream();
                    using (var zipStream = entry.Open())
                        zipStream.CopyTo(memoryStream);

                    var instance = Instance.FromStream(memoryStream);
                    instance.ToFile(outputFile);
                    var report = InstanceComparer.Report(instance, Instance.FromFile(outputFile));
                    File.WriteAllLines(Path.Combine(outputFolder, Path.ChangeExtension(entry.Name, "log")), report.Messages);
                    result.Add(report);

                }
            }
            return result;
        }
    }
}

