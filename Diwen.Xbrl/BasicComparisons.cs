////
//  This file is part of Diwen.xbrl.
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2018 John Nordberg
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

namespace Diwen.Xbrl
{
	using System;

	[Flags, Serializable]
	public enum BasicComparisons
	{
		None = 0,
		NullInstances = 1 << 0,
		SchemaReference = 1 << 1,
		Units = 1 << 2,
		FilingIndicators = 1 << 3,
		ContextCount = 1 << 4,
		FactCount = 1 << 5,
		DomainNamespaces = 1 << 6,
		Entity = 1 << 7,
		Period = 1 << 8,
		All = 0xFFFFFFF
	}
}