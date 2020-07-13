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
	public class DeleteArticleCommand : IRequest
	{
		public string Slug { get; set; }

		public class Handler : IRequestHandler<DeleteArticleCommand>
		{
			private readonly DbContext context;
			private readonly ICurrentUserService currentUser;

			public Handler(DbContext context, ICurrentUserService currentUser)
			{
				this.context = context;
				this.currentUser = currentUser;
			}

			public async Task<Unit> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
			{
				var article = await context.Set<Article>()
					.Include(a => a.Tags)
					.Include(a => a.Author)
					.SingleOrDefaultAsync(a => a.Slug == request.Slug, cancellationToken);

				if (article == null)
				{
					throw new EntityNotFoundException<Article>(request.Slug);
				}

				if (article.Author.Email != this.currentUser.Email)
				{
					throw new BadRequestException("Only author can delete article");
				}

				context.RemoveRange(article.Tags);
				context.Remove(article);
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
