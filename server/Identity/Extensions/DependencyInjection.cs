using System;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Extensions
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddIdentity(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
		{
			services
				.AddDbContext<ApplicationIdentityDbContext>(optionsAction);

			services
				.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationIdentityDbContext>();

			services
				.AddScoped<IUserService, IdentityUserService>();

			return services;
		}
	}
}
