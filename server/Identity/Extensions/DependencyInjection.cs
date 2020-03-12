using System;
using Application.Common.Interfaces;
using Identity.Configuration;
using Identity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Extensions
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddIdentity<TCurrentUserService>(this IServiceCollection services,
			Action<DbContextOptionsBuilder> optionsAction,
			Action<AuthConfiguration> authConfigurationAction)
			where TCurrentUserService : class, ICurrentUserService
		{
			services
				.AddDbContext<ApplicationIdentityDbContext>(optionsAction);

			var configuration = new AuthConfiguration();
			authConfigurationAction.Invoke(configuration);

			services
				.AddSingleton(configuration)
				.AddSingleton<JwtTokenService>();

			services
				.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationIdentityDbContext>();

			services
				.AddScoped<IUserService, IdentityUserService>()
				.AddScoped<ICurrentUserService, TCurrentUserService>();

			return services;
		}
	}
}
