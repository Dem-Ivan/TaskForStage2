using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApplicationAPI15_SecondStageTS_.Models;
using WebApplicationAPI15_SecondStageTS_.utils.Paging;
using WebApplicationAPI15_SecondStageTS_.utils.Sort;

namespace WebApplicationAPI15_SecondStageTS_.utils
{
	public static class SortExtension
	{
        //public static IQueryable<T> SortAndPaging<T>(IQueryable<T> Myobjects,
        //     string? sortName, SortMode? sortMode, int page, int pageSize) where T : IQueryable<Announcement>
        //{
        //    Myobjects = Myobjects.OrderBy<T, string>(sortName);

        //    if (sortMode == SortMode.Desc)
        //    {
        //        Myobject = sortName switch
        //        {
        //            SortName.OrderNumber => Myobject.OrderByDescending(o => o.OrderNumber),
        //            SortName.Rating => Myobject.OrderByDescending(o => o.Rating),
        //            _ => Myobject.OrderByDescending(o => o.CreationDate)
        //        };
        //    }
        //    else
        //    {
        //        Myobject = sortName switch
        //        {
        //            SortName.OrderNumber => Myobject.OrderBy(o => o.OrderNumber),
        //            SortName.Rating => Myobject.OrderBy(o => o.Rating),
        //            _ => Myobject.OrderBy(o => o.CreationDate)
        //        };
        //    }

        //    return (Myobjects);
        //}

        public static IQueryable<T> GetSortBy<T, K>(IQueryable<T> Myobjects,Expression<Func<T, K>> sortExpr)
        {
            Myobjects = Myobjects.OrderBy(sortExpr);
            return Myobjects;
        }
    }
}
