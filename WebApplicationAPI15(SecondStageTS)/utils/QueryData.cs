using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationAPI15_SecondStageTS_.utils.Sort;

namespace WebApplicationAPI15_SecondStageTS_.utils
{
	public class QueryData
	{		
		public string? searchString { get; set;}

		public Guid? FilterByUserId { get; set; }

		public string? sortName { get; set; } = "CreationDate";
	
		public SortDirection? sortDirection { get; set; } = SortDirection.Asc;
	}
}
