//
//  ContextsTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2018 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests
{
    using System.IO;
    using Xunit;

    public static class ContextTests
    {

        // 			<xbrli:context id="end2014">
        //     <xbrli:entity>
        //       <xbrli:identifier scheme="http://www.xbrl.fi">1234567-1</xbrli:identifier>
        //     </xbrli:entity>
        //     <xbrli:period>
        //       <xbrli:instant>2014-12-31</xbrli:instant>
        //     </xbrli:period>
        //   </xbrli:context>


        [Fact]
        public static void CompareContextsWithSameEntity()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var left = new Context();
            left.Entity = entity;
            var right = new Context();
            right.Entity = entity;

            Assert.Equal(left, right);
        }

        [Fact]
        public static void CompareContextsWithDifferentEntity()
        {
            var leftentity = new Entity("http://www.xbrl.fi", "1234567-1");
            var rightentity = new Entity("http://www.xbrl.fi", "0123456-7");
            var left = new Context();
            left.Entity = leftentity;
            var right = new Context();
            right.Entity = rightentity;

            Assert.NotEqual(left, right);
        }

        [Fact]
        public static void CompareContextsWithSameInstant()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var period = new Period(2014, 12, 31);

            var left = new Context();
            left.Entity = entity;
            left.Period = period;

            var right = new Context();
            right.Entity = entity;
            right.Period = period;

            Assert.Equal(left, right);
        }

        [Fact]
        public static void CompareContextsWithDifferentInstant()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var leftperiod = new Period(2014, 12, 31);
            var rightperiod = new Period(2014, 11, 30);

            var left = new Context();
            left.Entity = entity;
            left.Period = leftperiod;

            var right = new Context();
            right.Entity = entity;
            right.Period = rightperiod;

            Assert.NotEqual(left, right);
        }

        [Fact]
        public static void CompareContextsWithSameSpan()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var period = new Period(2014, 12, 01, 2014, 12, 31);

            var left = new Context();
            left.Entity = entity;
            left.Period = period;

            var right = new Context();
            right.Entity = entity;
            right.Period = period;

            Assert.Equal(left, right);
        }

        [Fact]
        public static void CompareContextsWithDifferentSpan()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var leftperiod = new Period(2014, 12, 01, 2014, 12, 31);
            var rightperiod = new Period(2014, 11, 01, 2014, 11, 30);

            var left = new Context();
            left.Entity = entity;
            left.Period = leftperiod;

            var right = new Context();
            right.Entity = entity;
            right.Period = rightperiod;

            Assert.NotEqual(left, right);
        }

        [Fact]
        public static void CompareContextsWithDifferentTypeOfPeriod()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var leftperiod = new Period(2014, 12, 31);
            var rightperiod = new Period(2014, 12, 1, 2014, 12, 31);

            var left = new Context();
            left.Entity = entity;
            left.Period = leftperiod;

            var right = new Context();
            right.Entity = entity;
            right.Period = rightperiod;

            Assert.NotEqual(left, right);
        }

        //   <xbrli:context id="Context_Instant_IntangibleRights">
        //     <xbrli:entity>
        //       <xbrli:identifier scheme="http://www.xbrl.fi">1234567-1</xbrli:identifier>
        //       <xbrli:segment>
        //         <xbrldi:explicitMember dimension="fi-sbr-dim-D033:IntangibleAssetsBreakdownDimension">fi-sbr-dim-D033:IntangibleRights</xbrldi:explicitMember>
        //       </xbrli:segment>
        //     </xbrli:entity>
        //     <xbrli:period>
        //       <xbrli:instant>2013-12-31</xbrli:instant>
        //     </xbrli:period>
        //   </xbrli:context>

        [Fact]
        public static void CompareContextsWithSameSegment()
        {
            var entity = new Entity("http://www.xbrl.fi", "1234567-1");
            var period = new Period(2013, 12, 31);

            var segment = new Segment();
            segment.AddExplicitMember("dimPrefix:dimCode", "valuePrefix:valueCode");
            entity.Segment = segment;

            var left = new Context();
            left.Entity = entity;
            left.Period = period;



            var right = new Context();
            right.Entity = entity;
            right.Period = period;

            Assert.Equal(left, right);
        }

        [Fact]
        public static void CompareContextsWithDifferentSegment()
        {
            var period = new Period(2013, 12, 31);

            var leftentity = new Entity("http://www.xbrl.fi", "1234567-1");
            var leftsegment = new Segment();
            leftsegment.AddExplicitMember("dimPrefix:dimCode", "valuePrefix:valueCode");
            leftentity.Segment = leftsegment;

            var left = new Context();
            left.Entity = leftentity;
            left.Period = period;


            var rightentity = new Entity("http://www.xbrl.fi", "1234567-1");
            var rightsegment = new Segment();
            rightsegment.AddExplicitMember("dimPrefix:dimOtherCode", "valuePrefix:valueOtherCode");
            rightentity.Segment = rightsegment;

            var right = new Context();
            right.Entity = rightentity;
            right.Period = period;

            Assert.NotEqual(left, right);
        }

        // [Fact]
        // public static void AddLargeNumberOfContexts()
        // {
        //     var instance = Instance.FromFile(Path.Combine("data", "ars.xbrl"));

        //     var scenario = new Scenario();
        //     scenario.ExplicitMembers.Add("s2c_dim:BL", "s2c_LB:x142");
        //     scenario.ExplicitMembers.Add("s2c_dim:CS", "s2c_CS:x26");
        //     scenario.ExplicitMembers.Add("s2c_dim:LX", "s2c_GA:LU");
        //     scenario.ExplicitMembers.Add("s2c_dim:BL", "s2c_LB:x142");
        //     for (int i = 0; i < 10; i++)
        //     {
        //         var context = instance.GetContext()
        //     }

        // }


        // <xbrldi:explicitMember dimension="s2c_dim:PI">s2c_PI:x1</xbrldi:explicitMember>
        // <xbrldi:explicitMember dimension="s2c_dim:VG">s2c_AM:x80</xbrldi:explicitMember>
        // <xbrldi:explicitMember dimension="s2c_dim:VL">s2c_VM:x5</xbrldi:explicitMember>
    }
}
