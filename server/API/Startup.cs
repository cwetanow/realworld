using Application.Common.Extensions;
using Identity.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
				});

			services
				.AddPersistence(options => options.UseNpgsql(Configuration.GetConnectionString(typeof(ConduitDbContext).Name)))
				.AddIdentity(options => options.UseNpgsql(Configuration.GetConnectionString("IdentityDbContext")))
				.AddApplication();
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

			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});
		}
	}
}
