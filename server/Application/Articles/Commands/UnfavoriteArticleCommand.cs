using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Articles.Commands
{
	public class UnfavoriteArticleCommand : IRequest
	{
		public string Slug { get; set; }

		public class Handler : IRequestHandler<UnfavoriteArticleCommand>
		{
			private readonly DbContext context;
			private readonly ICurrentUserService currentUser;

			public Handler(DbContext context, ICurrentUserService currentUser)
			{
				this.context = context;
				this.currentUser = currentUser;
			}

			public async Task<Unit> Handle(UnfavoriteArticleCommand request, CancellationToken cancellationToken)
			{
				var favourite = await context.Set<FavouritedArticle>()
					.SingleOrDefaultAsync(a => a.Article.Slug == request.Slug && a.UserId == currentUser.UserId, cancellationToken);

				if (favourite is null)
				{
					throw new EntityNotFoundException<FavouritedArticle>($"User { currentUser.UserId } article {request.Slug}");
				}

				context.Remove(favourite);
				await context.SaveChangesAsync(cancellationToken);

				return Unit.Value;
			}
		}

		public class Validator : AbstractValidator<DeleteArticleCommand>
		{
			public Validator()
			{
				RuleFor(c => c.Slug).NotEmpty();
			}
		}
	}
}
