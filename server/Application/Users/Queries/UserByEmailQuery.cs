using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Users.Models;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries
{
	public class UserByEmailQuery : IRequest<UserWithProfileDto>
	{
		public string Email { get; set; }

		public class Handler : IRequestHandler<UserByEmailQuery, UserWithProfileDto>
		{
			private readonly DbContext context;
			private readonly IMapper mapper;

			public Handler(DbContext context, IMapper mapper)
			{
				this.context = context;
				this.mapper = mapper;
			}

			public async Task<UserWithProfileDto> Handle(UserByEmailQuery request, CancellationToken cancellationToken)
			{
				var userProfile = await context.Set<UserProfile>()
					.AsNoTracking()
					.SingleOrDefaultAsync(p => p.Email == request.Email, cancellationToken);

				if (userProfile == null)
				{
					throw new EntityNotFoundException<UserProfile>(request.Email);
				}

				return mapper.Map<UserWithProfileDto>(userProfile);
			}
		}

		public class Validator : AbstractValidator<UserByEmailQuery>
		{
			public Validator()
			{
				RuleFor(q => q.Email).NotEmpty();
			}
		}
	}
}
