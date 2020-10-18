using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using AutoMapper;
using MessageBoard.Options;
using Microsoft.Extensions.FileProviders;
using System.IO;
using MessageBoard.Context;
using MessageBoard.Services.RecaptchaService;
using MessageBoard.Repositoryes;
using MessageBoard.dto;

namespace MessageBoard
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
			services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Annoucement board", Version = "v1" }));

			services.Configure<ReCaptchaOptions>(Configuration.GetSection("ReCaptcha"));
			services.Configure<UserOptions>(Configuration);
			services.AddHttpClient<IRecaptchaService, GoogleRecaptchaService>();
			services.AddScoped<IRepository<AnnouncementRespons, AddAnntRequest, UpdateAnntRequest>, AnnouncementRepository>();
			services.AddScoped<IRepository<UserDto, UserDto, UserDto>, UserRepository>();
		}
		public void Configure(IApplicationBuilder app)
		{
			app.UseDeveloperExceptionPage();

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "Annoucement board V1");
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
