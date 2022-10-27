//
//  ScenarioTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2022 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests
{
    using Xunit;
    using Diwen.Xbrl;

    public static class SegmentTests
    {
        [Fact]
        public static void Bug_76()
        {
            string schemaPath = "http://tempuri.org/schema.xsd";
            var instance = new Instance();
            instance.SchemaReference = new SchemaReference("schema", schemaPath);
            instance.TaxonomyVersion = "1.0";

            instance.SetDimensionNamespace("ns1", "http://tempuri.org/xbrl/dim");
            instance.AddDomainNamespace("ns2", "http://tempuri.org/xbrl/dom");
            instance.AddDomainNamespace("ns3", "http://tempuri.org/xbrl/fact");

            instance.Entity = new Entity("http://tempuri.org/id", "12345678");
            instance.Period = new Period(2022, 09, 30);

            var segment = new Segment();
            segment.AddExplicitMember("ns1:AA", "ns1:aaa"); // Ok ("ns1" is used in the same segment as dimension)
            segment.AddExplicitMember("ns1:BB", "ns2:bbb"); // Exception ("ns2" is not used anywhere else but here)
            segment.AddExplicitMember("ns1:BB", "ns3:ccc"); // Ok ("ns3" is used in the fact name)                 

            instance.AddFact(segment, "ns3:metric", "", "2", "12345");

            instance.ToFile("output_segment_namespace.xbrl");
        }

    }

}