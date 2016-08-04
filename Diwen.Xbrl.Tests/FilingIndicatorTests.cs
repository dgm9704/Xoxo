//
//  FilingIndicatorTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2016 John Nordberg
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
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class FilingIndicatorTests
    {
        [Test]
        public void PositiveFilingIndicatorsMatchExactly()
        {
            var ctx = new Context();
            var a = new FilingIndicatorCollection();
            a.Add(ctx, "A.00.01", true);
            var b = new FilingIndicatorCollection();
            b.Add(ctx, "A.00.01", true);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void NegativeFilingIndicatorsMatchExactly()
        {
            var ctx = new Context();
            var a = new FilingIndicatorCollection();
            a.Add(ctx, "A.00.01", false);
            var b = new FilingIndicatorCollection();
            b.Add(ctx, "A.00.01", false);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void PositiveFilingIndicatorsMatchImplicitly()
        {
            var ctx = new Context();
            var a = new FilingIndicatorCollection();
            a.Add(ctx, "A.00.01", true);
            var b = new FilingIndicatorCollection();
            b.Add(ctx, "A.00.01");
            Assert.AreEqual(a, b);
        }

        [Test]
        public void FilingIndicatorCollectionsMatchFunctionally()
        {
            var c0 = new Context();
            var a = new FilingIndicatorCollection();
            a.Add(c0, "A", true);
            a.Add(c0, "B", true);
            a.Add(c0, "C", false);

            var c1 = new Context();
            var b = new FilingIndicatorCollection();
            b.Add(c1, "B");
            b.Add(c1, "A");

            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void FilingIndicatorCollectionsDoNotMatchFunctionally()
        {
            var c0 = new Context();
            var a = new FilingIndicatorCollection();
            a.Add(c0, "A", true);
            a.Add(c0, "B", true);
            a.Add(c0, "C", false);

            var c1 = new Context();
            var b = new FilingIndicatorCollection();
            b.Add(c1, "A");
            b.Add(c1, "B");
            b.Add(c1, "C");

            Assert.IsFalse(a.Equals(b));
        }
    }
}

