using System.Threading.Tasks;
using Application.Profiles.Commands;
using Application.Profiles.Models;
using Application.Profiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class ProfileController : BaseController
	{
		public ProfileController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost("{username}/follow")]
		public async Task<ProfileDto> FollowUser(string username)
		{
			await mediator.Send(new FollowUserCommand { Username = username });

			return await mediator.Send(new ProfileByUsernameQuery { Username = username });
		}
	}
}
