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
				var user = await context.Set<UserProfile>()
					.Include(c => c.FollowedUsers)
					.SingleOrDefaultAsync(c => c.Id == this.currentUser.UserId, cancellationToken);

				var otherUser = await context.Set<UserProfile>()
					.SingleOrDefaultAsync(up => up.Username == request.Username, cancellationToken);

				if (otherUser is null)
				{
					throw new EntityNotFoundException<UserProfile>(request.Username);
				}

				if (user.HasFollowed(otherUser))
				{
					throw new BadRequestException("Current user already follows the other user");
				}

				user.Follow(otherUser);
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
