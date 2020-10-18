using MessageBoard.dto;
using MessageBoard.utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBoard.Repositoryes
{
	public interface IRepository<T, TU, TV>
		where T : class
		where TU : class
		where TV : class
	{
		Task<GetResult<T>> GetObjectList(QueryData queryData, int page, int pageSize, CancellationToken cancellationToken);
		Task<T> GetObject(Guid Id, CancellationToken cancellationToken);
		Task<Guid> CreateObject(TU item, CancellationToken cancellationToken);
		Task<Guid> UpdateObject(TV item, Guid Id, CancellationToken cancellationToken);
		Task<Guid> DeleteObject(Guid Id, CancellationToken cancellationToken);
	}
}
