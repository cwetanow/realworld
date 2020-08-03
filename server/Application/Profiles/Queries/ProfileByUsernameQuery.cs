using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Profiles.Models;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Profiles.Queries
{
	public class ProfileByUsernameQuery : IRequest<ProfileDto>
	{
		public string Username { get; set; }

		public class Handler : IRequestHandler<ProfileByUsernameQuery, ProfileDto>
		{
			private readonly DbContext context;
			private readonly IMapper mapper;
			private readonly ICurrentUserService currentUser;

			public Handler(DbContext context, IMapper mapper, ICurrentUserService currentUser)
			{
				this.context = context;
				this.mapper = mapper;
				this.currentUser = currentUser;
			}

			public async Task<ProfileDto> Handle(ProfileByUsernameQuery request, CancellationToken cancellationToken)
			{
				var profile = await context.Set<UserProfile>()
					.AsNoTracking()
					.SingleOrDefaultAsync(p => p.Username == request.Username, cancellationToken);

				if (profile == null)
				{
					throw new EntityNotFoundException<UserProfile>(request.Username);
				}

				var dto = mapper.Map<ProfileDto>(profile);

				if (currentUser.IsAuthenticated)
				{
					dto.Following = await context.Set<UserFollower>()
						.AnyAsync(f => f.UserId == profile.Id && f.FollowerId == currentUser.UserId, cancellationToken);
				}

				return dto;
			}
		}

		public class Validator : AbstractValidator<ProfileByUsernameQuery>
		{
			public Validator()
			{
				RuleFor(q => q.Username).NotEmpty();
			}
		}
	}
}
