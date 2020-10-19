using System;

namespace MessageBoard.ProgectExeptions
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
