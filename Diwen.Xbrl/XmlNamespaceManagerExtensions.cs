//
//  This file is part of Diwen.Xbrl.
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
using System.Linq;

namespace Diwen.Xbrl
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public static class XmlNamespaceManagerExtensions
    {
        public static XmlSerializerNamespaces ToXmlSerializerNamespaces(this Instance instance)
        {
            if(instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            List<string> usedDomains = instance.GetUsedDomainNamespaces();

            var result = new XmlSerializerNamespaces();
            foreach(var item in Instance.DefaultNamespaces)
            {
                result.Add(item.Key, item.Value);
            }

            var foo = new List<string>();

            if(instance.Facts.Any())
            {
                foo.Add(instance.FactNamespace);
            }

            var scenarios = instance.Contexts.Where(c => c.Scenario != null).Select(c => c.Scenario).ToList();

            if(scenarios.Any(s => s.TypedMembers.Any()))
            {
                foo.Add(instance.DimensionNamespace);
                foo.Add(instance.TypedDomainNamespace);
            }
            else if(scenarios.Any(s => s.ExplicitMembers.Any()))
            {
                foo.Add(instance.DimensionNamespace);
            }

            foreach(var item in foo)
            {
                if(!string.IsNullOrEmpty(item))
                {
                    var prefix = instance.Namespaces.LookupPrefix(item);
                    result.Add(prefix, item);
                }
            }

            foreach(var item in usedDomains)
            {
                result.Add(instance.Namespaces.LookupPrefix(item), item);
            }
            return result;
        }
    }
}