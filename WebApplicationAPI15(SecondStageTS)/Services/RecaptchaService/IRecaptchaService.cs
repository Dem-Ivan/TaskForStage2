using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MessageBoard.Services.RecaptchaService
{
	public interface IRecaptchaService
	{
		 Task<RecaptchaResponse> Validate(IFormCollection form);
	}
}
