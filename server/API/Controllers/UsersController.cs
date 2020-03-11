using System.Threading.Tasks;
using API.Requests;
using Application.Users.Commands.AuthenticateUser;
using Application.Users.Commands.RegisterUser;
using Application.Users.Models;
using Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class UsersController : BaseController
	{
		private readonly JwtTokenService tokenService;

		public UsersController(IMediator mediator, JwtTokenService tokenService)
			: base(mediator)
		{
			this.tokenService = tokenService;
		}

		[HttpPost]
		public async Task<UserWithProfileDto> RegisterUser([FromBody] UserRequest<RegisterUserCommand> request) => await mediator.Send(request.User);

		[HttpPost("login")]
		public async Task<UserWithProfileDto> AuthenticateUser([FromBody] UserRequest<AuthenticateUserCommand> request)
		{
			var dto = await mediator.Send(request.User);

			dto.Token = tokenService.CreateToken(dto.Email);

			return dto;
		}
	}
}
