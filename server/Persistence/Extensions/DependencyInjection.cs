using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence.Extensions
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddPersistence(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
		{
			services
				.AddDbContext<ConduitDbContext>(optionsAction)
				.AddScoped<DbContext>(provider => provider.GetService<ConduitDbContext>());

			return services;
		}
	}
}
