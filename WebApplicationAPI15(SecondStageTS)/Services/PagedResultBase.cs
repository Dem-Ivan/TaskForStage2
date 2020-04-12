using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationAPI15_SecondStageTS_.dto;
using WebApplicationAPI15_SecondStageTS_.Models;

namespace WebApplicationAPI15_SecondStageTS_.Services
{
	public abstract class PagedResultBase
	{
		public int CurrentPage { get; set; }
		public int PageCount { get; set; }
		public int PageSize { get; set; }
		public int RowCount { get; set; }

		public int FirstRowOnPage 
		{
			get { return (CurrentPage - 1) * PageSize + 1; }
		}

		public int LastRowOnPage
		{
			get { return Math.Min(CurrentPage * PageSize, RowCount); }
		}
	}

	public class PagedResult<T> : PagedResultBase where T : class
	{
		public IQueryable<T> Results { get; set; }	
		
	}
}
