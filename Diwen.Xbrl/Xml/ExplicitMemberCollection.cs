//
//  This file is part of Diwen.xbrl.
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

    /// <summary/>
    public class ExplicitMemberCollection : Collection<ExplicitMember>, IEquatable<IList<ExplicitMember>>
    {
        Report reportField;

        /// <summary/>
        [XmlIgnore]
        public Report Report
        {
            get { return reportField; }
            set
            {
                reportField = value;

                for (int i = 0; i < this.Count; i++)
                {
                    var item = this[i];

                    item.Report = value;

                    if (string.IsNullOrEmpty(item.Dimension.Namespace))
                    {
                        var dimensionNs = reportField.DimensionNamespace;
                        item.Dimension = new XmlQualifiedName(item.Dimension.LocalName(), dimensionNs);
                    }

                    if (string.IsNullOrEmpty(item.Value.Namespace))
                    {
                        string valNs = Report.Namespaces.LookupNamespace(item.Value.Prefix());

                        if (!string.IsNullOrEmpty(valNs))
                        {
                            if (item.Value.Namespace != valNs)
                                item.Value = new XmlQualifiedName(item.Value.LocalName(), valNs);
                        }
                        else if (Report.CheckExplicitMemberDomainExists)
                        {
                            throw new InvalidOperationException($"No namespace declared for domain '{item.Value.Prefix()}'");
                        }
                    }
                    this[i] = item;
                }
            }
        }

        /// <summary/>
        public ExplicitMemberCollection()
        {
        }

        /// <summary/>
        public ExplicitMemberCollection(Report report)
            : this()
        {
            Report = report;
        }

        /// <summary/>
        public ExplicitMember Add(string dimension, string value)
        {
            if (string.IsNullOrEmpty(dimension))
                throw new ArgumentOutOfRangeException(nameof(dimension));

            if (string.IsNullOrEmpty(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            XmlQualifiedName dim;
            XmlQualifiedName val;

            if (Report != null)
            {
                var dimPrefix = 
                    dimension.IndexOf(':') == -1
                    ? string.Empty
                    : dimension.Split(':').First();

                string dimNs =
                    string.IsNullOrEmpty(dimPrefix)
                    ? Report.DimensionNamespace
                    : Report.Namespaces.LookupNamespace(dimPrefix);

                var valPrefix = value.Substring(0, value.IndexOf(':'));
                var valNs = Report.Namespaces.LookupNamespace(valPrefix);
                value = value.Substring(value.IndexOf(':') + 1);
                if (Report.CheckExplicitMemberDomainExists)
                    if (string.IsNullOrEmpty(valNs))
                        throw new InvalidOperationException($"No namespace declared for domain '{valPrefix}'");

                dim = new XmlQualifiedName(dimension, dimNs);
                val = new XmlQualifiedName(value, valNs);
            }
            else
            {
                dim = new XmlQualifiedName(dimension);
                val = new XmlQualifiedName(value);
            }

            var explicitMember = new ExplicitMember(dim, val);
            Add(explicitMember);
            return explicitMember;
        }

        int hashCode = -1;

        /// <summary/>
        public override int GetHashCode()
        {
            if (hashCode == -1)
                hashCode = this.
                            Select(m => m.Value.LocalName()).
                            OrderBy(m => m).
                            Join("").
                            GetHashCode();

            return hashCode;
        }

        #region IEquatable implementation

        /// <summary/>
        public bool Equals(IList<ExplicitMember> other)
        => this.ContentCompare(other);

        #endregion
    }
}