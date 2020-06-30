using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplicationAPI15_SecondStageTS_.dto;
using WebApplicationAPI15_SecondStageTS_.utils.Sort;
using System.Diagnostics.CodeAnalysis;

namespace WebApplicationAPI15_SecondStageTS_.utils.Paging
{
	public static class QueryableExtension
	{       
        public static  PagedResult<T> GetPaged<T>(this IQueryable<T> objects, int page, int pageSize) where T : class
        {
            var pagedResult = new PagedResult<T>();
            pagedResult.CurrentPage = page;
            pagedResult.PageSize = pageSize;
            pagedResult.RowCount = objects.Count();

            var pageCoutnt = (double)pagedResult.RowCount / pageSize;
            pagedResult.PageCount = (int)Math.Ceiling(pageCoutnt);

            var skip = (page - 1) * pageSize;
            pagedResult.Result = objects.Skip(skip).Take(pageSize);         
            return pagedResult;

        }
        public static IQueryable<T> GetSortBy<T, K>(this IQueryable<T> objects,Expression<Func<T, K>> sortExpr, SortDirection? sortDirection)
        {
            if (sortDirection == SortDirection.Desc)
            {
                objects = objects.OrderByDescending(sortExpr);
            }
            else
            {
                objects = objects.OrderBy(sortExpr);
            }
           
            return objects;
        }

        public static async Task<IEnumerable<T>> MappingTo<T>([NotNull]this IQueryable objects, IMapper mapper)
        {
            if (mapper==null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            if (objects == null)
            {
                throw new ArgumentNullException(nameof(objects));
            }
            return await mapper.ProjectTo<T>(objects).ToListAsync();
        }
    }
}
