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
	public class UnfollowUserCommand : IRequest
	{
		public string Username { get; set; }

		public class Handler : IRequestHandler<UnfollowUserCommand>
		{
			private readonly ICurrentUserService currentUser;
			private readonly DbContext context;

			public Handler(ICurrentUserService currentUser, DbContext context)
			{
				this.currentUser = currentUser;
				this.context = context;
			}

			public async Task<Unit> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
			{
				var user = await context.Set<UserProfile>()
					.Include(c => c.FollowedUsers)
					.SingleOrDefaultAsync(c => c.Id == this.currentUser.UserId, cancellationToken);

				var otherUser = await context.Set<UserProfile>()
					.SingleOrDefaultAsync(up => up.Username == request.Username, cancellationToken);

				if (otherUser is null)
				{
					throw new EntityNotFoundException<UserProfile>(request.Username);
				}

				if (!user.HasFollowed(otherUser))
				{
					throw new EntityNotFoundException<UserFollower>(new { UserId = otherUser.Id, FollowerId = user.Id });
				}

				user.Unfollow(otherUser);

				await context.SaveChangesAsync(cancellationToken);

				return Unit.Value;
			}
		}

		public class Validator : AbstractValidator<UnfollowUserCommand>
		{
			public Validator()
			{
				RuleFor(c => c.Username).NotEmpty();
			}
		}
	}
}
