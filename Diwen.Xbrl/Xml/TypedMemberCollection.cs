//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2024 John Nordberg
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

namespace Diwen.Xbrl.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using Diwen.Xbrl.Extensions;

    public class TypedMemberCollection : Collection<TypedMember>, IEquatable<IList<TypedMember>>
    {
        Report reportField;

        [XmlIgnore]
        public Report Report
        {
            get { return reportField; }
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                reportField = value;
                var dimNs = reportField.DimensionNamespace;
                var dimPrefix = reportField.Namespaces.LookupPrefix(dimNs);
                var domNs = reportField.TypedDomainNamespace;
                var domprefix = reportField.Namespaces.LookupPrefix(domNs);

                for (int i = 0; i < this.Count; i++)
                {
                    var item = this[i];
                    var dirty = false;
                    item.Report = value;
                    if (item.Dimension.Namespace != reportField.DimensionNamespace)
                    {
                        item.Dimension = new XmlQualifiedName($"{dimPrefix}:{item.Dimension.Name}", dimNs);
                        dirty = true;
                    }

                    if (item.Domain.Namespace != reportField.TypedDomainNamespace)
                    {
                        item.Domain = new XmlQualifiedName($"{domprefix}:{item.Domain.Name}", domNs);
                        dirty = true;
                    }

                    if (dirty)
                        this[i] = item;
                }
            }
        }

        public TypedMemberCollection()
        {
        }

        public TypedMemberCollection(Report report)
            : this()
        {
            Report = report;
        }

        public TypedMember Add(string dimension, string domain, string value)
        {

            XmlQualifiedName dim;
            XmlQualifiedName dom;
            if (Report != null)
            {
                dim = new XmlQualifiedName(dimension, Report.DimensionNamespace);
                dom = new XmlQualifiedName(domain, Report.TypedDomainNamespace);
            }
            else
            {
                dim = new XmlQualifiedName(dimension);
                dom = new XmlQualifiedName(domain);
            }

            var typedMember = new TypedMember(dim, dom, value);
            Add(typedMember);
            return typedMember;
        }

        #region IEquatable implementation

        public bool Equals(IList<TypedMember> other)
        => this.ContentCompare(other);

        #endregion

        int hashCode = -1;

        public override int GetHashCode()
        {
            if (hashCode == -1)
                hashCode = this.Select(m => m.Value ?? "").
                    OrderBy(m => m).
                    Join("").
                    GetHashCode();

            return hashCode;
        }
    }
}