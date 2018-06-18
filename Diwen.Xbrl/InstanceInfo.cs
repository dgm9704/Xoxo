//
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
	using System.Collections.Generic;

	struct InstanceInfo
	{
		public string TaxonomyVersion { get; }

		public string InstanceGenerator { get; }

		public List<string> Comments { get; }

		public InstanceInfo(string taxonomyVersion, string instanceGenerator, List<string> comments)
			: this()
		{
			TaxonomyVersion = taxonomyVersion;
			InstanceGenerator = instanceGenerator;
			Comments = comments;
		}
	}
}

