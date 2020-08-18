using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
