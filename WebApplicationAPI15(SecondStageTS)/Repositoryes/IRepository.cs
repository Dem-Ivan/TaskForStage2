using MessageBoard.dto;
using MessageBoard.utils;
using System;
using System.Threading.Tasks;

namespace MessageBoard.Repositoryes
{
	interface IRepository<T, TKey>  where T : class
	{
		Task<GetResult<T>> GetList(QueryData queryData, int page, int pageSize = 25);
		Task<T> Get(Guid Id);
		void Create(TKey item, Guid Id);
		void Update(TKey item, Guid Id);
		void Delete(Guid Id);		
	}
}
