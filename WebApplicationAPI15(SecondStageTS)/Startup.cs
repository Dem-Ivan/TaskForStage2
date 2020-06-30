using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using AutoMapper;
using WebApplicationAPI15_SecondStageTS_.Options;
using Microsoft.Extensions.FileProviders;
using System.IO;
using WebApplicationAPI15_SecondStageTS_.Context;
using WebApplicationAPI15_SecondStageTS_.Services.RecaptchaService;

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

			services.Configure<ReCaptchaOptions>(Configuration.GetSection("ReCaptcha"));
			services.Configure<UserOptions>(Configuration);			
			services.AddHttpClient<IRecaptchaService, GoogleRecaptchaService>();
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

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
					Path.Combine(Directory.GetCurrentDirectory(), @Configuration["BigImagesDirectory"])),
				RequestPath = new Microsoft.AspNetCore.Http.PathString("/BigImages")
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(
					Path.Combine(Directory.GetCurrentDirectory(), @Configuration["SmallImagesDirectory"])),
				RequestPath = new Microsoft.AspNetCore.Http.PathString("/SmallImages")
			});

			app.UseRouting();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}
