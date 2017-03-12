//
//  ExternalTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2016 John Nordberg
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

namespace Diwen.Xbrl.Tests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
	public static class ExternalTests
	{
		//[Test]
		//[Ignore("bad performance")]
		//public static void EBA()
		//{
		//	//CheckFolderResults(TestFolder("eba"));
		//	var folder = Path.Combine(TestContext.CurrentContext.TestDirectory, "eba");
		//	foreach (var zip in Directory.GetFiles(folder, "*.zip"))
		//	{
		//		TestZippedFiles(zip, folder);
		//	}
		//}

		//[Test]
		//[Ignore("bad performance")]
		//public static void EIOPA()
		//{
		//	CheckFolderResults(TestFolder("eiopa"));
		//}

		[Test]
		public static void Fi_Sbr()
		{
			CheckFolderResults(TestFolder("fi-sbr"));
		}

		static void CheckFolderResults(Dictionary<string, ComparisonReport> reports)
		{
			Assert.IsTrue(
				reports.Values.All(report => report.Result),
				string.Join(Environment.NewLine,
					reports.
					Where(report => !report.Value.Result).
					Select(report => report.Key)));
		}

		static Dictionary<string, ComparisonReport> TestFolder(string folderName)
		{
			return Directory.GetFiles(Path.Combine(TestContext.CurrentContext.TestDirectory, folderName), "*.xbrl").
				ToDictionary(inputFile => inputFile,
							inputFile => TestFile(inputFile,
												Path.ChangeExtension(inputFile, "out"),
												Path.ChangeExtension(inputFile, "log")));
		}

		static ComparisonReport TestFile(string inputFile, string outputFile, string reportFile)
		{
			Instance.FromFile(inputFile).ToFile(outputFile);
			var report = InstanceComparer.Report(inputFile, outputFile);
			File.WriteAllLines(reportFile, report.Messages);
			return report;
		}

		static List<ComparisonReport> TestZippedFiles(string zipFile, string outputFolder)
		{
			var result = new List<ComparisonReport>();
			using (var file = File.OpenRead(zipFile))
			using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
			{
				foreach (var entry in zip.Entries.Where(e => Path.GetExtension(e.Name) == ".xbrl"))
				{
					var outputFile = Path.Combine(outputFolder, Path.ChangeExtension(entry.Name, "out"));

					var memoryStream = new MemoryStream();
					using (var zipStream = entry.Open())
					{
						zipStream.CopyTo(memoryStream);
					}
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

