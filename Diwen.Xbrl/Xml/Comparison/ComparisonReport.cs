﻿//
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

namespace Diwen.Xbrl.Xml.Comparison
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class ComparisonReport
    {
        public bool Result { get; }

        public ReadOnlyCollection<string> Messages { get; }

        internal ComparisonReport(bool result, IList<string> messages)
        {
            Result = result;
            Messages = new ReadOnlyCollection<string>(messages);
        }

        public override string ToString()
        {
            return $"result: {Result}" + (
                Result
                ? string.Empty
                : Environment.NewLine + string.Join(Environment.NewLine, Messages));
        }
    }
}

