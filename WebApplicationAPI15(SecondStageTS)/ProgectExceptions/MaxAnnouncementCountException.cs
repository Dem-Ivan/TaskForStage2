using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationAPI15_SecondStageTS_.ProgectExeptions
{
	public class MaxAnnouncementCountException : Exception
	{
		

		public MaxAnnouncementCountException()
		{
		}
		public MaxAnnouncementCountException(string message) : base(message)
		{

		}
		public MaxAnnouncementCountException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
