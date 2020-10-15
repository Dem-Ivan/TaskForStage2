using MessageBoard.dto;
using MessageBoard.utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBoard.Repositoryes
{
	public interface IUserRepository<T, TKey>
		where T : class
		where TKey : class
	{
		Task<GetResult<T>> GetObjectList(QueryData queryData, int page, int pageSize, CancellationToken cancellationToken);
		Task<T> GetObject(Guid Id, CancellationToken cancellationToken);
		Task<Guid> CreateObject(TKey item, CancellationToken cancellationToken);
		Task<Guid> UpdateObject(TKey item, Guid Id, CancellationToken cancellationToken);
		Task<Guid> DeleteObject(Guid Id, CancellationToken cancellationToken);
	}
}
