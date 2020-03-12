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
				var currentUserEmail = currentUser.Email;

				var currentUserId = await context.Set<UserProfile>()
					.AsNoTracking()
					.Where(up => up.Email == currentUserEmail)
					.Select(u => u.Id)
					.SingleAsync(cancellationToken);

				var otherUserId = await context.Set<UserProfile>()
					.AsNoTracking()
					.Where(up => up.Username == request.Username)
					.Select(u => u.Id)
					.SingleOrDefaultAsync(cancellationToken);

				if (otherUserId is default(int))
				{
					throw new EntityNotFoundException<UserProfile>(request.Username);
				}

				var following = await context.Set<UserFollower>()
					.SingleOrDefaultAsync(f => f.UserId == otherUserId && f.FollowerId == currentUserId, cancellationToken);

				if (following == null)
				{
					throw new EntityNotFoundException<UserFollower>(new { UserId = otherUserId, FollowerId = currentUserId });
				}

				context.Remove(following);
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
