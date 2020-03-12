using API.Infrastructure.Extensions;
using API.Infrastructure.Services;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Identity.Extensions;
using Identity.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services
				.AddMvcCore(opts => {
					opts.OutputFormatters.Add(new HttpNoContentOutputFormatter());
					opts.Filters.Add(new ProducesAttribute("application/json"));
					opts.Filters.Add(new AuthorizeFilter("default"));
				});

			services
				.ConfigureConduitJwtAuthentication()
				.ConfigureAuthorization();

			services
				.AddPersistence(options => options.UseNpgsql(Configuration.GetConnectionString(typeof(ConduitDbContext).Name)))
				.AddIdentity<HttpContextCurrentUserService>(options => options.UseNpgsql(Configuration.GetConnectionString("IdentityDbContext")),
					authConfiguration => Configuration.Bind("Auth", authConfiguration))
				.AddApplication();

			services
				.AddHttpContextAccessor();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});
		}
	}
}
