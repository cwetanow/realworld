using System.Linq;
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
    public class FavouriteArticleCommand : IRequest
    {
        public string Slug { get; set; }

        public class Handler : IRequestHandler<FavouriteArticleCommand>
        {
            private readonly DbContext context;
            private readonly ICurrentUserService currentUser;

            public Handler(DbContext context, ICurrentUserService currentUser)
            {
                this.context = context;
                this.currentUser = currentUser;
            }

            public async Task<Unit> Handle(FavouriteArticleCommand request, CancellationToken cancellationToken)
            {
                var article = await context.Set<Article>()
                    .Include(a => a.Author)
                    .SingleOrDefaultAsync(a => a.Slug == request.Slug, cancellationToken: cancellationToken);

                if (article == null)
                {
                    throw new EntityNotFoundException<Article>(request.Slug);
                }

                var user = await context.Set<UserProfile>()
                    .SingleOrDefaultAsync(u => u.Email == currentUser.Email, cancellationToken);

                var isArticleFavourited = await context.Set<FavouritedArticle>()
                    .AnyAsync(a => a.ArticleId == article.Id && a.UserId == user.Id, cancellationToken);

                if (isArticleFavourited)
                {
                    throw new BadRequestException("Article is already favourited");
                }

                var newFavouriteArticle = new FavouritedArticle(article, user);

                context.Add(newFavouriteArticle);
                await context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<FavouriteArticleCommand>
        {
            public Validator()
            {
                RuleFor(c => c.Slug).NotEmpty();
            }
        }
    }
}