using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using WebApplicationAPI15_SecondStageTS_.Models;
using AutoMapper;
using Microsoft.Extensions.FileProviders;
using System.IO;
using WebApplicationAPI15_SecondStageTS_.Options;
using WebApplicationAPI15_SecondStageTS_.Services;

namespace WebApplicationAPI15_SecondStageTS_
{
	public class Startup
	{	
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}
		public IConfiguration Configuration { get; set; }
		public void ConfigureServices(IServiceCollection services)
		{
			string connection = Configuration.GetConnectionString("DefaultConnection");
			services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));
			services.AddControllers();			

			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			services.AddSwaggerGen(c => c.SwaggerDoc("v1",new OpenApiInfo { Title = "My API", Version = "v1"}));

			services.Configure<AppOptions>(Configuration);
			services.AddSingleton<IRecaptchaService, GoogleRecaptchaService>();
		}
		public void Configure(IApplicationBuilder app)
		{
			app.UseDeveloperExceptionPage();
		
			app.UseSwagger();
			app.UseSwaggerUI(c=>
			{
				c.SwaggerEndpoint(url:"/swagger/v1/swagger.json", name:"My API V1");
			});

			app.UseDefaultFiles();
			app.UseStaticFiles();
			
			app.UseRouting();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}
