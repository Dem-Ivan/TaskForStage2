using System.Collections.Generic;


namespace MessageBoard.utils.Paging
{
	public class PagedResult<T> : PagedResultBase where T : class
	{
		//public IQueryable<T> Result { get; set; }
		public IEnumerable<T> Result { get; set; } //T - AnnouncementRespons
	}
}

