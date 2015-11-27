//
//  This file is part of Diwen.xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015 John Nordberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Diwen.Xbrl
{
	using System;
	using System.Diagnostics;
	using System.Xml.Schema;
	using System.Xml.Serialization;

	[DebuggerDisplay("{Value} : {Filed}")]
	[Serializable]
	[XmlRoot("filingIndicator", Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
	public class FilingIndicator : IEquatable<FilingIndicator>
	{
		[XmlAttribute("contextRef")]
		public string ContextRef { get; set; }

		[XmlAttribute(AttributeName = "filed", Form = XmlSchemaForm.Qualified,
			Namespace = "http://www.eurofiling.info/xbrl/ext/filing-indicators")]
		public bool Filed { get; set; }

		[XmlText]
		public string Value { get; set; }

		private Context context;

		[XmlIgnore]
		public Context Context
		{
			get { return context; }
			set
			{
				context = value;
				ContextRef = context.Id;
			}
		}

		public FilingIndicator()
		{
			this.Filed = true;
		}

		public FilingIndicator(Context context, string value)
			: this(context, value, true)
		{
		}

		public FilingIndicator(Context context, string value, bool filed)
			: this()
		{
			if(context == null)
			{
				throw new ArgumentNullException("context");
			}

			this.Context = context;
			this.Value = value;
			this.Filed = filed;
		}

		public override bool Equals(object obj)
		{
			var other = obj as FilingIndicator;
			if(other != null)
			{
				return this.Equals(other);
			}
			else
			{
				return base.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode() ^ this.Filed.GetHashCode();
		}

		#region IEquatable implementation

		public bool Equals(FilingIndicator other)
		{
			return other != null
			&& this.Filed == other.Filed
			&& this.Value.Equals(other.Value, StringComparison.Ordinal);
		}

		#endregion
	}
}