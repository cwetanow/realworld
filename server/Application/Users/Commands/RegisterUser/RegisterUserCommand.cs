using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.RegisterUser
{
	public class RegisterUserCommand : IRequest<UserWithProfileDto>
	{
		public string Username { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }

		public class Handler : IRequestHandler<RegisterUserCommand, UserWithProfileDto>
		{
			private readonly DbContext context;
			private readonly IMapper mapper;
			private readonly IUserService userService;

			public Handler(IUserService userService, IMapper mapper, DbContext context)
			{
				this.userService = userService;
				this.mapper = mapper;
				this.context = context;
			}

			public async Task<UserWithProfileDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
			{
				var (result, userId) = await userService.CreateUser(request.Username, request.Email, request.Password);

				if (!result.Success)
				{
					throw new BadRequestException(result.Errors);
				}

				var userProfile = new UserProfile(userId, request.Email, request.Username);

				context.Set<UserProfile>().Add(userProfile);
				await context.SaveChangesAsync(cancellationToken);

				return mapper.Map<UserWithProfileDto>(userProfile);
			}
		}
	}
}
