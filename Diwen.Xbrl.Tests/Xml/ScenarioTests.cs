//
//  ScenarioTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2026 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests.Xml
{
    using System;
    using System.IO;
    using Xunit;
    using Diwen.Xbrl.Xml.Comparison;
    using Diwen.Xbrl.Xml;

    public static class ScenarioTests
    {
        [Fact]
        public static void CompareScenarioMemberOrderDifferent()
        {
            var left = Report.FromFile(Path.Combine("data","xml", "memberorder0.xbrl"));
            var right = Report.FromFile(Path.Combine("data","xml", "memberorder1.xbrl"));

            Assert.Equal(left, right);

            var report = ReportComparer.Report(left, right);

            Assert.True(report.Result, string.Join(Environment.NewLine, report.Messages));

        }
    }
}
