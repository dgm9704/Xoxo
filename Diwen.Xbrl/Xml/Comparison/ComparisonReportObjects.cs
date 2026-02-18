//
//  This file is part of Diwen.Xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2026 John Nordberg
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

namespace Diwen.Xbrl.Xml.Comparison
{
    using System;
    using System.Collections.Generic;

    /// <summary/>
    public class ComparisonReportObjects
    {
        /// <summary/>
        public bool Result { get; internal set; }

        /// <summary/>
        public List<string> Basics { get; internal set; }

        /// <summary/>
        public Tuple<List<Scenario>, List<Scenario>> Contexts { get; internal set; }

        /// <summary/>
        public Tuple<List<Fact>, List<Fact>> Facts { get; internal set; }

        /// <summary/>
        public Tuple<List<string>, List<string>> DomainNamespaces { get; internal set; }

        /// <summary/>
        public Tuple<List<Unit>, List<Unit>> Units { get; internal set; }

        /// <summary/>
        public Tuple<List<Identifier>, List<Identifier>> Entities { get; internal set; }

        /// <summary/>
        public Tuple<List<Period>, List<Period>> Periods { get; internal set; }

        /// <summary/>
        public Tuple<List<string>, List<string>> TaxonomyVersions { get; internal set; }

        /// <summary/>
        public Tuple<List<SchemaReference>, List<SchemaReference>> SchemaReferences { get; internal set; }

        /// <summary/>
        public Tuple<List<FilingIndicator>, List<FilingIndicator>> FilingIndicators { get; internal set; }
    }
}
