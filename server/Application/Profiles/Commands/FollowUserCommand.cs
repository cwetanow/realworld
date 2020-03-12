using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Profiles.Commands
{
	public class FollowUserCommand : IRequest
	{
		public string Username { get; set; }

		public class Handler : IRequestHandler<FollowUserCommand>
		{
			private readonly ICurrentUserService currentUser;
			private readonly DbContext context;

			public Handler(ICurrentUserService currentUser, DbContext context)
			{
				this.currentUser = currentUser;
				this.context = context;
			}

			public async Task<Unit> Handle(FollowUserCommand request, CancellationToken cancellationToken)
			{
				var currentUserEmail = this.currentUser.Email;

				var currentUserId = await context.Set<UserProfile>()
					.AsNoTracking()
					.Where(up => up.Email == currentUserEmail)
					.Select(u => u.Id)
					.SingleOrDefaultAsync(cancellationToken);

				var otherUserId = await context.Set<UserProfile>()
					.AsNoTracking()
					.Where(up => up.Username == request.Username)
					.Select(u => u.Id)
					.SingleOrDefaultAsync(cancellationToken);

				if (otherUserId is default(int))
				{
					throw new EntityNotFoundException<UserProfile>(request.Username);
				}

				var isUserFollower = await context.Set<UserFollower>()
					.AnyAsync(f => f.UserId == otherUserId && f.FollowerId == currentUserId, cancellationToken);

				if (isUserFollower)
				{
					throw new BadRequestException("Current user already follows the other user");
				}

				var newFollower = new UserFollower(otherUserId, currentUserId);
				context.Set<UserFollower>().Add(newFollower);
				await context.SaveChangesAsync(cancellationToken);

				return Unit.Value;
			}
		}

		public class Validator : AbstractValidator<FollowUserCommand>
		{
			public Validator()
			{
				RuleFor(c => c.Username).NotEmpty();
			}
		}
	}
}
