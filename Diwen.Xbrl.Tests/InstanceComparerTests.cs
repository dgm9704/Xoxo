//
//  InstanceComparerTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.Xbrl.Tests
{
	using System;
	using NUnit.Framework;
	using System.IO;

	[TestFixture]
	public class InstanceComparerTests
	{
		[Test]
		public static void CompareInstanceToItself()
		{
			var path = Path.Combine("data", "reference.xbrl");
			var firstInstance = Instance.FromFile(path);
			var secondInstance = Instance.FromFile(path);
			var report = InstanceComparer.Report(firstInstance, secondInstance);
			Assert.IsTrue(report.Result);
			CollectionAssert.IsEmpty(report.Messages);
		}

		[Test]
		public static void CompareTotallyDifferentInstances()
		{
			var firstPath = Path.Combine("data", "reference.xbrl");
			var secondPath = Path.Combine("data", "ars.xbrl");
			var firstInstance = Instance.FromFile(firstPath);
			var secondInstance = Instance.FromFile(secondPath);
			var report = InstanceComparer.Report(firstInstance, secondInstance);
			Assert.IsFalse(report.Result);
			CollectionAssert.IsNotEmpty(report.Messages);

			Console.WriteLine(string.Join(Environment.NewLine, report.Messages));
		}
	}
}

