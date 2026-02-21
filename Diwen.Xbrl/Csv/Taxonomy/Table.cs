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
    using System.Text.Json.Serialization;
    using System.Text.RegularExpressions;

    /// <summary/>
    public class Table : EsaDocumented
    {

        /// <summary/>
        [JsonPropertyName("optional")]
        public bool Optional { get; set; }

        /// <summary/>
        [JsonPropertyName("template")]
        public string Template { get; set; }

        /// <summary/>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        private string filingIndicator;


        private static string letterVariantPattern = @"-[a-z]";
        private static Regex letterVariantExpression = new(letterVariantPattern, RegexOptions.Compiled);

        /// <summary />
        [JsonIgnore]
        public string FilingIndicator
        {
            get
            {
                filingIndicator ??=
                    EsaDocumentation != null && EsaDocumentation.ContainsKey("FilingIndicator")
                    ? EsaDocumentation["FilingIndicator"].ToString()
                    //: string.Join('.', Template.Split('-').Take(2)); 
                    : string.Join('.', letterVariantExpression.Replace(Template, string.Empty).Split('-'));

                return filingIndicator;
            }
        }
    }
}