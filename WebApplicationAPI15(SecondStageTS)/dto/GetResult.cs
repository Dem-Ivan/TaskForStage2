using System.Collections.Generic;

namespace WebApplicationAPI15_SecondStageTS_.dto
{
	public struct  GetResult<T>
	{		
		public IEnumerable<T> Rows { get; set; }
		public int CountRowsFound { get; set; }
	}
}
