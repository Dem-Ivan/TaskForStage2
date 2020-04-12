using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplicationAPI15_SecondStageTS_.Models;
using WebApplicationAPI15_SecondStageTS_.Services;

namespace WebApplicationAPI15_SecondStageTS_
{
	public class Program
	{
		public static void Main(string[] args)
		{		
			var host = CreateWebHostBuilder(args).Build();
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;

				try
				{
					var context = services.GetRequiredService<ApplicationContext>();
					SimpleData.Initialize(context);
				}
				catch (Exception)
				{
					var loger = services.GetRequiredService<ILogger<Program>>();					
				}
			}
			host.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
