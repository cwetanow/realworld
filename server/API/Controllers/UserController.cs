using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Users.Models;
using Application.Users.Queries;
using Identity.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class UserController : BaseController
	{
		private readonly ICurrentUserService currentUser;

		public UserController(IMediator mediator, ICurrentUserService currentUser) : base(mediator)
		{
			this.currentUser = currentUser;
		}

		[HttpGet("")]
		public async Task<UserWithProfileDto> GetCurrentUser() => await mediator.Send(new UserByEmailQuery { Email = currentUser.Email });
	}
}
