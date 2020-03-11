using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Identity.Exceptions;
using Identity.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Identity
{
	public class IdentityUserService : IUserService
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly TokenService tokenService;

		public IdentityUserService(UserManager<ApplicationUser> userManager, TokenService tokenService)
		{
			this.userManager = userManager;
			this.tokenService = tokenService;
		}

		public async Task<(Result result, string userId)> CreateUser(string username, string email, string password)
		{
			var user = new ApplicationUser(username, email);

			var result = await userManager.CreateAsync(user, password);

			return (result.ToApplicationResult(), user.Id);
		}

		public async Task<string> Authenticate(string email, string password)
		{
			var user = await userManager.FindByEmailAsync(email);

			if (user == null)
			{
				throw new InvalidCredentialsException();
			}

			var isPasswordCorrect = await userManager.CheckPasswordAsync(user, password);

			if (!isPasswordCorrect)
			{
				throw new InvalidCredentialsException();
			}

			var token = tokenService.CreateToken(user.UserName, user.Id);

			return token;
		}
	}
}
