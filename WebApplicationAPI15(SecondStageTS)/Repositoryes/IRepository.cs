using MessageBoard.dto;
using MessageBoard.utils;
using System;
using System.Threading.Tasks;

namespace MessageBoard.Repositoryes
{
	public interface IRepository<T, TKey>  where T : class
	{
		Task<GetResult<T>> GetObjectList(QueryData queryData, int page, int pageSize = 25);
		Task<T> GetObject(Guid Id);
		Task<Guid> CreateObject(TKey item, Guid Id);
		Task<Guid> UpdateObject(TKey item, Guid Id);
		Task<Guid> DeleteObject(Guid Id);		
	}
}
