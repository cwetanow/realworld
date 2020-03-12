using System.Threading.Tasks;
using API.Requests;
using Application.Users.Commands.AuthenticateUser;
using Application.Users.Commands.RegisterUser;
using Application.Users.Models;
using Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

	[Route("api/users")]
	[AllowAnonymous]
	public class IdentityController : BaseController
	{
		private readonly JwtTokenService tokenService;

		public IdentityController(IMediator mediator, JwtTokenService tokenService)
			: base(mediator)
		{
			this.tokenService = tokenService;
		}

		[HttpPost]
		public async Task<UserWithProfileDto> RegisterUser([FromBody] UserRequest<RegisterUserCommand> request) => await ExecuteCommandAndReturnUserWithToken(request.User);

		[HttpPost("login")]
		public async Task<UserWithProfileDto> AuthenticateUser([FromBody] UserRequest<AuthenticateUserCommand> request) => await ExecuteCommandAndReturnUserWithToken(request.User);

		private async Task<UserWithProfileDto> ExecuteCommandAndReturnUserWithToken<TCommand>(TCommand command)
			where TCommand : IRequest<UserWithProfileDto>
		{
			var dto = await mediator.Send(command);

			dto.Token = tokenService.CreateToken(dto.Email);

			return dto;
		}
	}
}
