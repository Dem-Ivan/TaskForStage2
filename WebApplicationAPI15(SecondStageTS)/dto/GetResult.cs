using System.Collections.Generic;

namespace MessageBoard.dto
{
	public class GetResult<T>
	{
		public IEnumerable<T> Rows { get; set; }
		public int CountRowsFound { get; set; }
	}
}
