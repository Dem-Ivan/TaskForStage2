using System;

namespace MessageBoard.ProgectExceptions
{
	public class ReCaptchaErrorException : Exception
	{
		public ReCaptchaErrorException()
		{
		}

		public ReCaptchaErrorException(string message) : base(message)
		{
		}

		public ReCaptchaErrorException(string message, Exception innerException) : base(message, innerException)
		{
		}		
	}
}
