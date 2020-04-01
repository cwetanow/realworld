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
    public class UpdateArticleCommand : IRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }

        public string Slug { get; set; }

        public class Handler : IRequestHandler<UpdateArticleCommand>
        {
            private readonly ICurrentUserService currentUser;
            private readonly DbContext context;

            public Handler(DbContext context, ICurrentUserService currentUser)
            {
                this.context = context;
                this.currentUser = currentUser;
            }

            public async Task<Unit> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
            {
                var article = await context.Set<Article>()
                    .Include(a => a.Author)
                    .SingleOrDefaultAsync(a => a.Slug == request.Slug, cancellationToken: cancellationToken);

                if (article.Author.Email != currentUser.Email)
                {
                    throw new BadRequestException("User must be author to edit article");
                }

                if (article == null)
                {
                    throw new EntityNotFoundException<Article>(request.Slug);
                }

                article.Update(request.Title, request.Description, request.Body);

                context.Update(article);
                await context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<UpdateArticleCommand>
        {
            public Validator()
            {
                RuleFor(a => a.Slug).NotEmpty();
            }
        }
    }
}