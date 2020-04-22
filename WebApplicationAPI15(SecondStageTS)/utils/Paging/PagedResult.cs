using System.Linq;

namespace WebApplicationAPI15_SecondStageTS_.utils.Paging
{
	public class PagedResult<T> : PagedResultBase where T : class
	{
		public IQueryable<T> Results { get; set; }
	}
}
