//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2020 John Nordberg
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

namespace Diwen.Xbrl
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public static class InlineXbrl
    {
        public static Instance ParseInstance(XDocument report)
        {
            // Create new XBRL Instance
            var instance = new Instance();

            // parse namespaces from xhtml and add to instance
            var namespaces =
                report.
                Root.
                Attributes().
                Where(a => a.IsNamespaceDeclaration).
                Where(a=> a.Name.LocalName != "xmlns").
                ToDictionary(
                    g => g.Name.LocalName,
                    g => g.Value);

            foreach (var ns in namespaces)
                instance.Namespaces.AddNamespace(ns.Key, ns.Value);

            // parse schemaRef and add to instance
            // ix:header/ix:references/link:schemaRef
            var link = report.Root.GetNamespaceOfPrefix("link");
            // there can be only one
            var schemaRefElements = report.Root.Descendants(link + "schemaRef");
            var schemaRefSerializer = new XmlSerializer(typeof(SchemaReference));
            var schemaRefElement = schemaRefElements.Single();
            var schemaReader = schemaRefElement.CreateReader();
            var schemaRef = (SchemaReference)schemaRefSerializer.Deserialize(schemaReader);
            instance.SchemaReference = schemaRef;

            // parse contexts and add to instance
            // ix:header/ix:resources/xbrli:context
            var xbrli = report.Root.GetNamespaceOfPrefix("xbrli");
            var contextElements = report.Root.Descendants(xbrli + "context");
            var contextSerializer = new XmlSerializer(typeof(Context));

            var contexts = new ContextCollection(instance);
            foreach (var contextElement in contextElements)
            {
                var contextReader = contextElement.CreateReader();
                var context = (Context)contextSerializer.Deserialize(contextReader);
                contexts.Add(context);
            }
            instance.Contexts = contexts;

            // parse units and add to instance
            // ix:header/ix:resources/xbrli:unit
            var unitElements = report.Root.Descendants(xbrli + "unit");
            var unitSerializer = new XmlSerializer(typeof(Unit));
            var units = new UnitCollection(instance);
            foreach (var unitElement in unitElements)
            {
                var unitReader = unitElement.CreateReader();
                var unit = (Unit)unitSerializer.Deserialize(unitReader);
                units.Add(unit);
            }
            instance.Units = units;

            // parse facts and add to instance
            var factElements = report.Descendants().Where(d => d.Attribute("contextRef") != null);
            var facts = new FactCollection(instance);
            foreach (var factElement in factElements)
            {
                var name = factElement.Attribute("name").Value;
                var prefix = name.Split(':').First();
                var ns = report.Root.GetNamespaceOfPrefix(prefix).ToString();
                var unitRef = factElement.Attribute("unitRef")?.Value;
                var decimals = factElement.Attribute("decimals")?.Value;
                var value = factElement.Value;
                var scale = factElement.Attribute("scale")?.Value;

                if (!string.IsNullOrWhiteSpace(scale))
                {
                    var power = int.Parse(scale);
                    var v = decimal.Parse(value.Replace(" ", ""));
                    var multiplier = (decimal)Math.Pow(10, power);
                    v *= multiplier;
                    value = v.ToString();
                }

                var contextRef = factElement.Attribute("contextRef").Value;
                var fact = new Fact(name, ns, unitRef, decimals, contextRef, value);
                facts.Add(fact);
            }
            foreach(var fact in facts)
                instance.Facts.Add(fact);

            return instance;
        }

        public static Instance ParseInstance(string path)
        => ParseInstance(XDocument.Load(path));
    }
}