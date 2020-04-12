using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebApplicationAPI15_SecondStageTS_.Models;

namespace WebApplicationAPI15_SecondStageTS_.Services
{
	public interface IRecaptchaService
	{
		 Task<RecaptchaResponse> Validate(IFormCollection form);
	}
}
