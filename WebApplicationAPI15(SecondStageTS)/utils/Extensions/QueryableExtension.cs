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
using System.Threading;

namespace MessageBoard.utils.Paging
{
	public static class QueryableExtension
	{
		public static async Task<PagedResult<T>> GetPaged<T, TKey>(this IQueryable<TKey> objects, int page, int pageSize, IMapper mapper, CancellationToken cancellationToken)
			where T : class
			where TKey : class
		{
			var pagedResult = new PagedResult<T>();
			pagedResult.CurrentPage = page;
			pagedResult.PageSize = pageSize;
			pagedResult.RowCount = objects.Count();

			var pageCount = (double)pagedResult.RowCount / pageSize;
			pagedResult.PageCount = (int)Math.Ceiling(pageCount);
			var skip = (page - 1) * pageSize;

			pagedResult.Result = await mapper.ProjectTo<T>(objects.Skip(skip).Take(pageSize)).ToListAsync(cancellationToken);
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

			objects = objects.Where(s =>
				EF.Functions.Like(s.Text.ToUpper(), $"%{searchString}%") ||
				EF.Functions.Like(s.User.Name.ToUpper(), $"%{searchString}%") ||
				EF.Functions.Like(s.OrderNumber.ToString(), $"%{searchString}%") ||
				EF.Functions.Like(s.Rating.ToString(), $"%{searchString}%") ||
				EF.Functions.Like(s.CreationDate.ToString(), $"%{searchString}%")
				//(EF.Functions.Like(s.CreationDate.ToString("d", DateTimeFormatInfo.InvariantInfo), $"%{searchString}%"))
				);

			return objects;
		}
	}
}
