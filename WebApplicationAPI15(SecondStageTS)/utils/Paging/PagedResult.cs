using System.Linq;

namespace MessageBoard.utils.Paging
{
	public class PagedResult<T> : PagedResultBase where T : class
	{
		public IQueryable<T> Result { get; set; }
	}
}
