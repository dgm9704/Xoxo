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

namespace Diwen.Xbrl.Extensions
{
    using System.Collections.Generic;
    using Diwen.Xbrl.Csv.Taxonomy;

    /// <summary/>
    public static class ReportExtensions
    {
        /// <summary/>
        public static Csv.Report ToXbrlCsv(
            this Xml.Report xmlreport,
            Dictionary<string, TableDefinition> tableDefinitions,
            Dictionary<string, string> filingIndicators,
            ModuleDefinition moduleDefinition)
        => Csv.Report.FromXbrlXml(xmlreport, tableDefinitions, filingIndicators, moduleDefinition);

    }
}