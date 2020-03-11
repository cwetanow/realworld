using System.Threading.Tasks;
using API.Requests;
using Application.Users.Commands.RegisterUser;
using Application.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class UsersController : BaseController
	{
		public UsersController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost]
		public async Task<UserWithProfileDto> RegisterUser([FromBody] UserRequest<RegisterUserCommand> request) => await mediator.Send(request.User);
	}
}
