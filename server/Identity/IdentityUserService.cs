using System;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Identity.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Identity
{
	public class IdentityUserService : IUserService
	{
		private readonly UserManager<ApplicationUser> userManager;

		public IdentityUserService(UserManager<ApplicationUser> userManager)
		{
			this.userManager = userManager;
		}

		public async Task<(Result result, string userId)> CreateUser(string username, string email, string password)
		{
			var user = new ApplicationUser(username, email);

			var result = await userManager.CreateAsync(user, password);

			return (result.ToApplicationResult(), user.Id);
		}
	}
}
