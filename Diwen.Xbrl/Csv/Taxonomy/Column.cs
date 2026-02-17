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

namespace Diwen.Xbrl.Csv.Taxonomy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;

    /// <summary/>
    public class Column : EsaDocumented
    {
        /// <summary/>
        [JsonPropertyName("decimals")]
        public string Decimals { get; set; }

        /// <summary/>
        [JsonPropertyName("dimensions")]
        public Dictionary<string, string> Dimensions { get; set; } = [];

        private readonly HashSet<string> excludeDimensions = new(["concept", "unit"]);

        private Dictionary<string, string> dimensionValues;

        /// <summary/>
        public Dictionary<string, string> DimensionValues
        {
            get
            {
                dimensionValues ??=
                    Dimensions.Where(d => !excludeDimensions.Contains(d.Key)).ToDictionary(
                        d => d.Key,
                        d => d.Value);

                return dimensionValues;
            }
        }
    }
}