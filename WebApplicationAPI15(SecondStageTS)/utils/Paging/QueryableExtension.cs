using System;
using System.Linq;

namespace WebApplicationAPI15_SecondStageTS_.utils.Paging
{
	public static class QueryableExtension
	{       
        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query, int page, int pageSize) where T : class
        {
            var result = new PagedResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();

            var pageCoutnt = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCoutnt);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize);      
            
            return result;

        }
    }
}
