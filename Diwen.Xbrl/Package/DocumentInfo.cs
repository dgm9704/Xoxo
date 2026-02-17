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

namespace Diwen.Xbrl.Package
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary/>
    public class DocumentInfo
    {
        /// <summary/>
        [JsonPropertyName("extends")]
        public List<string> Extends { get; set; }

        /// <summary/>
        [JsonPropertyName("final")]
        public Dictionary<string, bool> Final { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("features")]
        public Features Features { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("namespaces")]
        public Dictionary<string, Uri> Namespaces { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("linkTypes")]
        public Dictionary<string, Uri> LinkTypes { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("linkGroups")]
        public Dictionary<string, Uri> LinkGroups { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("taxonomy")]
        public Uri[] Taxonomy { get; set; }

        /// <summary/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("baseURL")]
        public string BaseUrl { get; set; }
    }
}