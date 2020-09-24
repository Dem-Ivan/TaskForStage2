using System.Collections.Generic;

namespace MessageBoard.dto
{
	public class GetResult<T>
	{
		public IEnumerable<T> Data { get; set; }
		public int Count { get; set; }
	}
}
