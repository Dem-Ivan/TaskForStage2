using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MessageBoard.Options;

namespace MessageBoard.Services.RecaptchaService
{
	public class GoogleRecaptchaService : IRecaptchaService
	{
		private readonly IOptions<ReCaptchaOptions> _reCaptchaOptions;
		private readonly HttpClient _httpClient;

		public GoogleRecaptchaService(IOptions<ReCaptchaOptions> reCaptchaOptions, HttpClient httpClient)
		{
			_httpClient = httpClient;
			_httpClient.BaseAddress = new Uri("http://www.google.com");			
			_reCaptchaOptions = reCaptchaOptions;
		}

		public async Task<RecaptchaResponse> Validate(IFormCollection form)
		{
			var gRecaptchaResponse = form["g-recaptcha-response"];
			var content = new FormUrlEncodedContent(new[]
			{ 
				new KeyValuePair<string, string>("secret",_reCaptchaOptions.Value.SecretKey),
				new KeyValuePair<string, string>("response", gRecaptchaResponse)
			});

			var response = await _httpClient.PostAsync("/recaptcha/api/siteverify", content);
			var resultContent = await response.Content.ReadAsStringAsync();
			var captchaResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(resultContent);

			return captchaResponse;

		}
	}
}
