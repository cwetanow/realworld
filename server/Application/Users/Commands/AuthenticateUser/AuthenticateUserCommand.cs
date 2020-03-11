using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.AuthenticateUser
{
	public class AuthenticateUserCommand : IRequest<UserWithProfileDto>
	{
		public string Email { get; set; }
		public string Password { get; set; }

		public class Handler : IRequestHandler<AuthenticateUserCommand, UserWithProfileDto>
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

			public async Task<UserWithProfileDto> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
			{
				var token = await userService.Authenticate(request.Email, request.Password);

				var userProfile = await context.Set<UserProfile>()
					.AsNoTracking()
					.SingleOrDefaultAsync(p => p.Email == request.Email, cancellationToken);

				if (userProfile == null)
				{
					throw new EntityNotFoundException<UserProfile>(request.Email);
				}

				var dto = mapper.Map<UserWithProfileDto>(userProfile);
				dto.Token = token;

				return dto;
			}
		}
	}
}
