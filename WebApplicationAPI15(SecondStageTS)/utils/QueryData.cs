using System;
using MessageBoard.utils.Sort;

namespace MessageBoard.utils
{
	public class QueryData
	{
		public string SearchString { get; set; }

		public Guid? FilterByUserId { get; set; }

		public string SortName { get; set; } = "CreationDate";

		public SortDirection? SortDirection { get; set; } = Sort.SortDirection.Asc;
	}
}
