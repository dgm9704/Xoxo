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

namespace Diwen.Xbrl.Xml
{
    using System;
    using System.Xml.Serialization;

    /// <summary/>
    [Serializable]
    [XmlRoot(ElementName = "period", Namespace = "http://www.xbrl.org/2003/instance")]
    public class Period : IEquatable<Period>
    {
        /// <summary/>
        [XmlElement(ElementName = "instant", DataType = "date", Namespace = "http://www.xbrl.org/2003/instance")]
        public DateTime Instant { get; set; }

        /// <summary/>
        [XmlElement(ElementName = "startDate", DataType = "date", Namespace = "http://www.xbrl.org/2003/instance")]
        public DateTime StartDate { get; set; }

        /// <summary/>
        [XmlElement(ElementName = "endDate", DataType = "date", Namespace = "http://www.xbrl.org/2003/instance")]
        public DateTime EndDate { get; set; }

        /// <summary/>
        public Period()
        {

        }

        /// <summary/>
        public Period(DateTime instant)
            : this()
        {
            Instant = instant;
        }

        /// <summary/>
        public Period(int year, int month, int day)
            : this()
        {
            Instant = new DateTime(year, month, day);
        }

        /// <summary/>
        public Period(DateTime startDate, DateTime endDate)
            : this()
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary/>
        public Period(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
            : this()
        {
            StartDate = new DateTime(startYear, startMonth, startDay);
            EndDate = new DateTime(endYear, endMonth, endDay);
        }

        /// <summary/>
        public bool ShouldSerializeInstant()
        => Instant != DateTime.MinValue;

        /// <summary/>
        public bool ShouldSerializeStartDate()
        => StartDate != DateTime.MinValue;

        /// <summary/>
        public bool ShouldSerializeEndDate()
        => EndDate != DateTime.MinValue;

        /// <summary/>
        public override bool Equals(object obj)
        => Equals(obj as Period);

        /// <summary/>
        public override string ToString()
        => ShouldSerializeInstant()
         ? $"Instant={Instant:yyyy-MM-dd}"
         : $"StartDate={StartDate:yyyy-MM-dd}, EndDate={EndDate:yyyy-MM-dd}";

        #region IEquatable implementation

        /// <summary/>
        public bool Equals(Period other)
        => other != null
            && Instant.Equals(other.Instant)
            && StartDate.Equals(other.StartDate)
            && EndDate.Equals(other.EndDate);

        /// <summary/>
        public override int GetHashCode()
        => Instant.Date.GetHashCode()
          + 7 * StartDate.GetHashCode()
          + 31 * EndDate.GetHashCode();

        #endregion
    }
}
