using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplicationAPI15_SecondStageTS_.dto;

namespace WebApplicationAPI15_SecondStageTS_.utils.Paging
{
	public class PagedResult<T> : PagedResultBase where T : class
	{	
		public IQueryable<T> Result { get; set; }		
	}
}
