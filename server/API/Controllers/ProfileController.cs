using System.Threading.Tasks;
using Application.Profiles.Commands;
using Application.Profiles.Models;
using Application.Profiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class ProfileController : BaseController
	{
		public ProfileController(IMediator mediator) : base(mediator)
		{ }

		[AllowAnonymous]
		[HttpGet("{username}")]
		public async Task<ProfileDto> GetProfile(string username) => await mediator.Send(new ProfileByUsernameQuery { Username = username });

		[HttpPost("{username}/follow")]
		public async Task<ProfileDto> FollowUser(string username) =>
			await ChangeUserFollowing(new FollowUserCommand { Username = username }, username);

		[HttpDelete("{username}/follow")]
		public async Task<ProfileDto> UnfollowUser(string username) =>
			await ChangeUserFollowing(new UnfollowUserCommand { Username = username }, username);

		private async Task<ProfileDto> ChangeUserFollowing(IRequest command, string username)
		{
			await mediator.Send(command);

			return await mediator.Send(new ProfileByUsernameQuery { Username = username });
		}
	}
}
