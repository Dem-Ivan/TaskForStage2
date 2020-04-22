using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApplicationAPI15_SecondStageTS_.Services.RecaptchaService
{
	public interface IRecaptchaService
	{
		 Task<RecaptchaResponse> Validate(IFormCollection form);
	}
}
