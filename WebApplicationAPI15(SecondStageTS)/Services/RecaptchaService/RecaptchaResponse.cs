using Newtonsoft.Json;
using System.Collections.Generic;


namespace MessageBoard.Services.RecaptchaService
{
	public class RecaptchaResponse
	{
		[JsonProperty("success")]
		public bool Success { get; set; }

		[JsonProperty("error-code")]
		public List<string> ErrorCodes { get; set; }
	}
}
