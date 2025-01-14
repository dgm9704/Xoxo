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
    using System;
    using System.Xml;
#if NETSTANDARD2_0
    using ArgumentNullException = Compat.ArgumentNullException;
#endif

    /// <summary/>
    public static class XmlQualifiedNameExtensions
    {
        /// <summary/>
        public static string LocalName(this XmlQualifiedName value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var result = string.Empty;

            string name = value.Name;
            if (!string.IsNullOrEmpty(name))
                result = name.Substring(name.IndexOf(':') + 1);

            return result;
        }

        /// <summary/>
        public static string Prefix(this XmlQualifiedName value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var result = string.Empty;

            string name = value.Name;
            if (!string.IsNullOrEmpty(name))
            {
                var idx = name.IndexOf(':');
                if (idx != -1)
                    result = name.Substring(0, idx);
            }

            return result;
        }
    }
}