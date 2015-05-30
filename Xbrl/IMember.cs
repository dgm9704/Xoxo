namespace Xoxo
{
	using System.Xml.Schema;
	using System.Xml;
	using System;
	using System.Xml.Serialization;
	using System.Collections.ObjectModel;

	public interface IMember
	{
		string Dimension { get; set; }

		string Value{ get; set; }
	}
}