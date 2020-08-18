using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MessageBoard.utils.Sort;
using MessageBoard.Models;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MessageBoard.utils.Paging
{
	public static class QueryableExtension
	{
		public static PagedResult<T> GetPaged<T>(this IQueryable<T> objects, int page, int pageSize) where T : class
		{
			var pagedResult = new PagedResult<T>();
			pagedResult.CurrentPage = page;
			pagedResult.PageSize = pageSize;
			pagedResult.RowCount = objects.Count();

			var pageCount = (double)pagedResult.RowCount / pageSize;
			pagedResult.PageCount = (int)Math.Ceiling(pageCount);

			var skip = (page - 1) * pageSize;
			pagedResult.Result = objects.Skip(skip).Take(pageSize);
			return pagedResult;

		}
		public static IQueryable<T> GetSortBy<T, TKey>(this IQueryable<T> objects, Expression<Func<T, TKey>> sortExpr, SortDirection? sortDirection)
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

		public static IQueryable<T> SearchForMatches<T>(this IQueryable<T> objects, string searchString) where T : Announcement
		{
			searchString = searchString.ToUpper();

			objects = objects.Where(s => (EF.Functions.Like(s.Text.ToUpper(), $"%{searchString}%")) ||
				(EF.Functions.Like(s.user.Name.ToUpper(), $"%{searchString}%")) || (EF.Functions.Like(s.OrderNumber.ToString(), $"%{searchString}%")) ||
				(EF.Functions.Like(s.Rating.ToString(), $"%{searchString}%")) || (EF.Functions.Like(s.CreationDate.ToString("d", DateTimeFormatInfo.InvariantInfo), $"%{searchString}%")));

			return objects;
		}


		public static async Task<IEnumerable<T>> MappingTo<T>([NotNull]this IQueryable objects, IMapper mapper)
		{
			if (mapper == null)
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
