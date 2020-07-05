using System.Collections.Generic;

namespace WebApplicationAPI15_SecondStageTS_.dto
{
	public class  GetResult<T>
	{		
		public IEnumerable<T> Rows { get; set; }
		public int CountRowsFound { get; set; }
	}
}
